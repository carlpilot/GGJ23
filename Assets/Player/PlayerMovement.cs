using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody body;
    [Header("Walking/Running")]
    public float maxSpeed = 5;
    public float acceleration = 10f;

    [Header("Wall Running")]
    public bool enableWallRunning = true;
    public float wallRunSpeedThresholdMultiplier = 0.8f;
    public float wallStickAcceleration = 0.1f;

    [Header("Airtime")]
    public float airAccelerationMultiplier = 0.1f;

    [Header("Sliding")]
    public bool enableSliding = true;
    public float slideHeight = 0.5f;
    public float slideAccelerationMultiplier = 0.05f;
    public float slideAnimationSpeed = 1f;
    public float slideSpeedThresholdMultiplier = 0.8f;
    public float slideSpeedThresholdExitMultiplier = 0.4f;
    public GameObject normalColliders;
    public GameObject slidingColliders;

    [Header("Ziplining")]
    public bool enableZiplining = true;
    public float ziplineSpeed = 10f;

    [Header("Environment Detection")]
    public Vector3 footOffset = new Vector3(0,-0.6f,0);
    public float footRange = 0.45f;
    public Vector3 wallOffset = new Vector3(0,0,0);
    public float wallRange = 0.6f;

    [Header("Jumping")]
    [Tooltip("The delay between jumps")]
    public float jumpDelay = 0.5f;
    [Tooltip("The velocity of each jump")]
    public float jumpVelocity = 5f;

    [Header("Camera Setup")]
    public GameObject cam;
    public float sensitivity = 1f;
    public Transform headPosition;
    public Transform slidingHeadPosition;

    // Header - State
    public bool isMovementEnabled {get;private set;}

    public bool isGrounded {get;private set;}
    PlayerSlideBoost currentSlideBoost;
    PlayerSlip currentSlip;
    public bool isTouchingWall {get;private set;}
    public bool isWallRunning {get;private set;}
    public bool isTouchingHead {get;private set;}

    public bool isSliding {get;private set;}

    Vector3 groundNormal = Vector3.up;

    Vector3 wallNormal;

    Vector3 inputVector;

    Zipline currentZipline;
    float currentZiplineDirection;

    float rotY;
    Vector3 camTargetLocalPosition;
    float blockSlideTimer;
    float blockJumpTimer;
    float blockWallRunTimer;
    float blockZiplineTimer;
    float blockAccelerationTimer;
    
    void Awake() {
        body = GetComponent<Rigidbody>();
        camTargetLocalPosition = headPosition.localPosition;
        Physics.queriesHitTriggers = false;
    }

    void Start(){
        StartCoroutine(JumpChecker());
        SetMovementEnabled(true);
    }

    
    void Update(){
        // Do environment checks
        UpdateGrounded();
        UpdateOnWall();
        UpdateOnHead();

        // Calculate floor forward
        var normalRot = Quaternion.FromToRotation(Vector3.up, groundNormal);
        var floorForward = normalRot*transform.forward;

        // Update head position
        cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, camTargetLocalPosition, slideAnimationSpeed*Time.deltaTime);

        // Update slide blocking timer
        blockSlideTimer -= Time.deltaTime;
        blockJumpTimer -= Time.deltaTime;
        blockWallRunTimer -= Time.deltaTime;
        blockZiplineTimer -= Time.deltaTime;
        blockAccelerationTimer -= Time.deltaTime;
        
        if (isMovementEnabled){
            // Check for mouse move
            var rotX = Input.GetAxis("Mouse X") * sensitivity;
            rotY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotY = Mathf.Clamp(rotY, -90, 90);
            cam.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
            transform.Rotate(0, rotX, 0);

            // Can't move on a zipline
            if (currentZipline == null){
                // Check inputs for wasd
                inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (inputVector.magnitude > 1) inputVector = inputVector.normalized;

                // Check for entering or exiting a slide
                if (enableSliding && !isSliding && blockSlideTimer <= 0 && Input.GetKey(KeyCode.LeftShift) && isGrounded && Vector3.Dot(floorForward, body.velocity) > slideSpeedThresholdMultiplier*maxSpeed){
                    SetSliding(true, floorForward);
                }
                if (isSliding && !isTouchingHead && (!Input.GetKey(KeyCode.LeftShift) || Vector3.Dot(floorForward, body.velocity) < slideSpeedThresholdExitMultiplier)){
                    SetSliding(false);
                }   
            } else{
                inputVector = Vector3.zero;
                SetSliding(false);
            }
        } else{
            inputVector = Vector3.zero;
            SetSliding(false);
        }
    }

    void FixedUpdate(){
        // Apply update velocity
        if (isMovementEnabled){
            var normalRot = Quaternion.FromToRotation(Vector3.up, groundNormal);
            var floorForward = normalRot*transform.forward;
            var floorRight = normalRot*transform.right;
            Debug.DrawRay(transform.position, floorForward, Color.red, Time.fixedDeltaTime);

            if (currentZipline == null){
                var transformedInputVector = normalRot*transform.TransformDirection(inputVector);
                Debug.DrawRay(transform.position, transformedInputVector, Color.blue, Time.fixedDeltaTime);

                var targetForwardAmount = Vector3.Dot(floorForward, transformedInputVector*maxSpeed);
                var targetRightAmount = Vector3.Dot(floorRight, transformedInputVector*maxSpeed);
                var actualForwardAmount = Vector3.Dot(floorForward, body.velocity);
                var actualRightAmount = Vector3.Dot(floorRight, body.velocity);

                if (isSliding && targetForwardAmount > 1) targetForwardAmount = 0;
                var velAdd = ((targetForwardAmount-actualForwardAmount)*floorForward + (targetRightAmount-actualRightAmount)*floorRight)*Time.fixedDeltaTime*acceleration;
                if (isSliding) velAdd *= slideAccelerationMultiplier;
                else if(!isGrounded) velAdd *= airAccelerationMultiplier;
                
                if (true||blockAccelerationTimer < 0){
                    if (currentSlip) velAdd *= currentSlip.slipMultiplier;
                    body.velocity += velAdd;

                    var flatVelocity = Vector3.ProjectOnPlane(body.velocity, Vector3.up);
                    if (blockWallRunTimer <= 0 && !isGrounded && isTouchingWall && !isSliding && enableWallRunning && flatVelocity.magnitude >= maxSpeed * wallRunSpeedThresholdMultiplier){
                        body.velocity = flatVelocity - wallNormal*wallStickAcceleration*Time.deltaTime;
                        isWallRunning = true;
                    } else{
                        isWallRunning = false;
                    }
                }
            } else{
                if (currentZipline.IsOnZipline(transform.position) && !(Input.GetKey(KeyCode.E) && blockZiplineTimer < 0)){
                    var corrective = currentZipline.GetOnZiplinePos(transform.position, -1.5f) - transform.position;
                    var speed = currentZipline.GetDirection() * currentZiplineDirection * ziplineSpeed;
                    body.velocity = speed + corrective*5f;
                } else{
                    currentZipline = null;
                    blockZiplineTimer = 0.5f;
                }
            }
        }
    }

    IEnumerator JumpChecker(){
        while (true){
            if (isMovementEnabled){
                // Check for jumps
                if (Input.GetKey(KeyCode.Space) && (isGrounded || isWallRunning) && !isSliding && blockJumpTimer <= 0){
                    // Block sliding for at least until we can also next jump
                    blockSlideTimer = jumpDelay;
                    // Jump
                    if (isGrounded){
                        if (Vector3.Dot(groundNormal, body.velocity) < 0) body.velocity = Vector3.ProjectOnPlane(body.velocity, groundNormal) + groundNormal*jumpVelocity;
                        else body.velocity += groundNormal*jumpVelocity;
                    } else if (isWallRunning){
                        body.velocity += Vector3.up*jumpVelocity + wallNormal*jumpVelocity;
                        blockWallRunTimer = 0.5f;
                    }
                    //body.velocity =  new Vector3(body.velocity.x, jumpVelocity, body.velocity.y);
                    yield return new WaitForSeconds(jumpDelay);
                } else{
                    // Wait till next frame
                    yield return null;
                }
            } else{
                // Wait till next frame
                yield return null;
            }
        }
    }

    void UpdateGrounded(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.TransformPoint(footOffset), footRange);
        foreach (var hitCollider in hitColliders){
            // We dont want to pick up any player colliders as the ground
            if (hitCollider.gameObject.layer != gameObject.layer){
                if (!isGrounded) blockAccelerationTimer = 0.25f;
                isGrounded = true;
                //LayerMask layers = Physics.AllLayers;
                //layers &= ~(1 << gameObject.layer);
                if (Physics.Raycast(transform.TransformPoint(footOffset), transform.up*-1, out var hit/*, layers*/)){
                    groundNormal = hit.normal;
                    currentSlideBoost = hit.collider.GetComponent<PlayerSlideBoost>();
                    currentSlip = hit.collider.GetComponent<PlayerSlip>();
                } else{
                    groundNormal = transform.up;
                    currentSlideBoost = null;
                    currentSlip = null;
                }
                return;
            }
        }
        groundNormal = transform.up;
        isGrounded = false;
        currentSlideBoost = null;
        currentSlip = null;
    }

    void UpdateOnHead(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.TransformPoint(Vector3.up), 0.5f);
        foreach (var hitCollider in hitColliders){
            // We dont want to pick up any player colliders as the ground
            if (hitCollider.gameObject.layer != gameObject.layer){
                isTouchingHead = true;
                return;
            }
        }
        isTouchingHead = false;
    }

    void UpdateOnWall(){
        /*Collider[] hitColliders = Physics.OverlapSphere(transform.TransformPoint(wallOffset), wallRange);
        foreach (var hitCollider in hitColliders){
            // We dont want to pick up any player colliders as the ground
            if (hitCollider.gameObject.layer != gameObject.layer){
                isTouchingWall = true;
                return;
            }
        }*/
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right*-1, out hit, wallRange, Physics.AllLayers) && hit.collider.gameObject.GetComponent<WallRunnable>()){
            isTouchingWall = true;
            wallNormal = hit.normal;
            return;
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, wallRange, Physics.AllLayers) && hit.collider.gameObject.GetComponent<WallRunnable>()){
            isTouchingWall = true;
            wallNormal = hit.normal;
            return;
        }
        isTouchingWall = false;
        wallNormal = Vector3.zero;
    }

    public void SetMovementEnabled(bool enabled){
        isMovementEnabled = enabled;
        if (enabled){
            Cursor.lockState = CursorLockMode.Locked;
        } else{
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void SetSliding(bool sliding, Vector3 forward){
        if (isSliding == sliding) return; // Dont do anything is this is already the correct value
        isSliding = sliding;
        if (isSliding){
            camTargetLocalPosition = slidingHeadPosition.localPosition;
            normalColliders.SetActive(false);
            slidingColliders.SetActive(true);
            StartCoroutine(AddDelayedSpeedBoost(forward));
            blockAccelerationTimer = 0.25f;
        } else{
            camTargetLocalPosition = headPosition.localPosition;
            normalColliders.SetActive(true);
            slidingColliders.SetActive(false);
            blockSlideTimer = 0.5f;
            blockAccelerationTimer = 0.25f;
        }
    }
    void SetSliding(bool sliding){ SetSliding(sliding, transform.forward);}

    IEnumerator AddDelayedSpeedBoost(Vector3 forward){
        yield return new WaitForSeconds(0.15f);
        if (isSliding) {
            var v = forward * 5f;
            if (currentSlideBoost) v *= currentSlideBoost.slideBoostMultiplier;
            body.velocity += v;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(footOffset), footRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(wallOffset), wallRange);
    }

    public void SetDirectionalVelocity(Vector3 direction, float speed){
        body.velocity = Vector3.ProjectOnPlane(body.velocity, direction) + direction * speed;
        blockJumpTimer = 0.15f;
    }

    public void ReflectVelocity(Vector3 normal){
        body.velocity = Vector3.Reflect(body.velocity, normal);
        blockJumpTimer = 0.15f;
    }

    public void SetCurrentZipline(Zipline z, float dir){
        if (currentZipline || !enableZiplining || blockZiplineTimer >= 0) return;
        currentZipline = z;
        currentZiplineDirection = dir;
        blockZiplineTimer = 0.5f;
    }

}
