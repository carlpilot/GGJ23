using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    float bounceTimer;
    // Update is called once per frame
    void Update()
    {
        bounceTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider col){
        if (bounceTimer <= 0){
            col.GetComponentInParent<PlayerMovement>().ReflectVelocity(transform.up);
            bounceTimer = 0.5f;
        }
    }
}
