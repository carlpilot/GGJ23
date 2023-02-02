using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayerOnTouch : MonoBehaviour
{
    public float bounceSpeed=5f;

    AudioSource audio;

    void Awake(){
        audio = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider col){
        if (col.transform.GetComponentInParent<PlayerMovement>()){
            col.transform.GetComponentInParent<PlayerMovement>().SetDirectionalVelocity(Vector3.up, bounceSpeed);
            audio.Play();
        }
    }
}
