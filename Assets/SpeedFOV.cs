using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFOV : MonoBehaviour
{

    public float speed;
    public float lastSpeed;

    public float currentFOV;
    public float targetFOV;

    // FOV settings
    public float baseFov = 70f;
    public float fastFov = 100f;

    // Aceleration transition
    public float acceleration = 20f;
    public float deceleration = 40f;
    public float transitionRate;

    // Threshold speed to change FOV
    public float threshold = 10f;

    void Start()
    {
        currentFOV = baseFov;
        targetFOV = currentFOV;
        transitionRate = acceleration;
    }

    void SpeedToFov(){
        

        // Deceleration => Return to normal FOV
        if (speed < threshold){
            lastSpeed = speed;
            targetFOV = baseFov;
            transitionRate = deceleration;
        }

        // Acceleration => Increase FOV
        else if (speed > threshold){
            lastSpeed = speed;
            targetFOV = fastFov;
            transitionRate = acceleration;
        }
    }

    void fovStep(){
        currentFOV = Mathf.MoveTowards(currentFOV, targetFOV, Time.deltaTime * transitionRate);
    }


    void SetFOV()
    {
        Camera.main.fieldOfView = currentFOV;
    }

    void Update()
    {


        // Get player speed - Vector3.Scale to ignore vertical movement
        speed = Vector3.Scale(GetComponent<Rigidbody>().velocity, new Vector3(1f, 0.5f, 1f)).magnitude; 

        SpeedToFov();
        fovStep();
        SetFOV();

    }
}
