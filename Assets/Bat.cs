using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public Transform points;
    public float speed = 10f;
    public float turnSpeed = 180f;

    Transform currentPoint;
    // Start is called before the first frame update
    void Start()
    {
        SelectRandomPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - currentPoint.position).magnitude < 1f){
            SelectRandomPoint();
        }

        transform.position += transform.forward*Time.deltaTime*speed;//Vector3.MoveTowards(transform.position, currentPoint.position, speed*Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(currentPoint.position - transform.position), turnSpeed*Time.deltaTime);
    }

    void SelectRandomPoint(){
        currentPoint = points.GetChild(Random.Range(0, points.childCount));
    }
}
