using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour {

    public Transform points;
    public float speed = 10f;
    public float turnSpeed = 180f;

    public float positionRandomness = 3.0f;
    [Range(0f, 0.9f)]
    public float speedRandomness = 0.2f;

    Vector3 currentPoint;

    void Start () {
        SelectRandomPoint ();
        speed *= 1 + Random.Range(-speedRandomness, speedRandomness);
    }

    void Update () {
        if ((transform.position - currentPoint).magnitude < 1f) {
            SelectRandomPoint ();
        }

        transform.position += transform.forward * Time.deltaTime * speed;//Vector3.MoveTowards(transform.position, currentPoint.position, speed*Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (currentPoint - transform.position), turnSpeed * Time.deltaTime);
    }

    void SelectRandomPoint () {
        currentPoint = points.GetChild (Random.Range (0, points.childCount)).position + Random.insideUnitSphere * positionRandomness;
    }
}