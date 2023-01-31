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

    public bool isMovementEnabled {get;private set;}
    public bool isGrounded {get;private set;}
    public bool isTouchingWall {get;private set;}

    Vector3 groundNormal = Vector3.up;

    Vector3 inputVector;

    float rotY;
    
    void Awake() {
        body = GetComponent<Rigidbody>();
    }

    void Start(){
        StartCoroutine(JumpChecker());
        SetMovementEnabled(true);
    }

    
    void Update(){
        if (isMovementEnabled){
            // Check for mouse move
            var rotX = Input.GetAxis("Mouse X") * sensitivity;
            rotY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotY = Mathf.Clamp(rotY, -90, 90);
            cam.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
            transform.Rotate(0, rotX, 0);

            // Check inputs for wasd
            inputVector = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) inputVector += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) inputVector -= Vector3.forward;
            if (Input.GetKey(KeyCode.A)) inputVector += Vector3.left;
            if (Input.GetKey(KeyCode.D)) inputVector -= Vector3.left;
        } else{
            inputVector = Vector3.zero;
        }
    }

    void FixedUpdate(){
        // Do environment checks
        UpdateGrounded();
        UpdateOnWall();

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

            var velAdd = ((targetForwardAmount-actualForwardAmount)*floorForward + (targetRightAmount-actualRightAmount)*floorRight)*Time.fixedDeltaTime*acceleration;
            if(!isGrounded) velAdd *= airAccelerationMultiplier;
            body.velocity += velAdd;
        }
    }

    IEnumerator JumpChecker(){
        while (true){
            if (isMovementEnabled){
                // Check for jumps
                if (Input.GetKey(KeyCode.Space) && isGrounded){
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
                } else{
                    groundNormal = transform.up;
                }
                return;
            }
        }
        groundNormal = transform.up;
        isGrounded = false;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(footOffset), footRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(wallOffset), wallRange);
    }
}
