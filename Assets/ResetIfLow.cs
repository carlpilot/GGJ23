using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIfLow : MonoBehaviour
{
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15) transform.position = startPos;
    }
}
