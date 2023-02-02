using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody body;
    [Header("Walking/Running")]
    public float maxSpeed = 5;
    public float acceleration = 10f;

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
    public bool isTouchingWall {get;private set;}

    public bool isSliding {get;private set;}

    Vector3 groundNormal = Vector3.up;

    Vector3 inputVector;

    float rotY;
    Vector3 camTargetLocalPosition;
    float blockSlideTimer;
    
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

        // Calculate floor forward
        var normalRot = Quaternion.FromToRotation(Vector3.up, groundNormal);
        var floorForward = normalRot*transform.forward;

        // Update head position
        cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, camTargetLocalPosition, slideAnimationSpeed*Time.deltaTime);

        // Update slide blocking timer
        blockSlideTimer -= Time.deltaTime;
        
        if (isMovementEnabled){
            // Check for mouse move
            var rotX = Input.GetAxis("Mouse X") * sensitivity;
            rotY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotY = Mathf.Clamp(rotY, -90, 90);
            cam.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
            transform.Rotate(0, rotX, 0);

            // Check inputs for wasd
            inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Check for entering or exiting a slide
            if (enableSliding && !isSliding && blockSlideTimer <= 0 && Input.GetKey(KeyCode.LeftShift) && isGrounded && Vector3.Dot(floorForward, body.velocity) > slideSpeedThresholdMultiplier*maxSpeed){
                SetSliding(true, floorForward);
            }
            if (isSliding && (!Input.GetKey(KeyCode.LeftShift) || Vector3.Dot(floorForward, body.velocity) < slideSpeedThresholdExitMultiplier)){
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
            body.velocity += velAdd;
        }
    }

    IEnumerator JumpChecker(){
        while (true){
            if (isMovementEnabled){
                // Check for jumps
                if (Input.GetKey(KeyCode.Space) && isGrounded && !isSliding){
                    // Stop sliding
                    if (isSliding){
                        SetSliding(false);
                    }
                    // Block sliding for at least until we can also next jump
                    blockSlideTimer = jumpDelay;
                    // Jump
                    body.velocity += groundNormal*jumpVelocity;
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
                isGrounded = true;
                //LayerMask layers = Physics.AllLayers;
                //layers &= ~(1 << gameObject.layer);
                if (Physics.Raycast(transform.TransformPoint(footOffset), transform.up*-1, out var hit/*, layers*/)){
                    groundNormal = hit.normal;
                    currentSlideBoost = hit.collider.GetComponent<PlayerSlideBoost>();
                } else{
                    groundNormal = transform.up;
                    currentSlideBoost = null;
                }
                return;
            }
        }
        groundNormal = transform.up;
        isGrounded = false;
        currentSlideBoost = null;
    }

    void UpdateOnWall(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.TransformPoint(wallOffset), wallRange);
        foreach (var hitCollider in hitColliders){
            // We dont want to pick up any player colliders as the ground
            if (hitCollider.gameObject.layer != gameObject.layer){
                isTouchingWall = true;
                return;
            }
        }
        isTouchingWall = false;
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
        } else{
            camTargetLocalPosition = headPosition.localPosition;
            normalColliders.SetActive(true);
            slidingColliders.SetActive(false);
            blockSlideTimer = 0.5f;
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
        body.velocity = Vector3.ProjectOnPlane(direction, body.velocity) + direction * speed;
    }
}
