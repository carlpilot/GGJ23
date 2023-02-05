using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFOV : MonoBehaviour
{

    public float speed;
    public float lastSpeed;

    public float currentFOV;
    public float targetFOV;

    // Sensitivity settings
    public float baseFov = 70f;
    public float fastFov = 100f;
    public float transitionRate = 6f;

    void Start()
    {
        currentFOV = 40f;
        targetFOV = currentFOV;
    }

    void SpeedToFov(){
        

        // Deceleration => Return to normal FOV
        if (speed < lastSpeed){
            lastSpeed = speed;
            targetFOV = baseFov;
        }
        // Acceleration => Increase FOV
        else if (speed > lastSpeed){
            lastSpeed = speed;
            targetFOV = fastFov;
        }
    }

    void fovStep(){
        currentFOV = Mathf.MoveTowards(currentFOV, targetFOV, Time.deltaTime * transitionRate * speed);
    }


    void SetFOV()
    {
        Camera.main.fieldOfView = currentFOV;
    }

    void Update()
    {

        speed = GetComponent<Rigidbody>().velocity.magnitude; // Get player speed

        SpeedToFov();
        fovStep();
        SetFOV();

    }
}
