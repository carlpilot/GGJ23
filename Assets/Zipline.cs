using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    public GameObject endA;
    public GameObject endB;

    public Vector3 GetDirection(){
        return (endB.transform.position - endA.transform.position).normalized;
    }

    void OnTriggerStay(Collider col){
        if (!Input.GetKey(KeyCode.E)) return;
        var pm = col.GetComponentInParent<PlayerMovement>();
        if (!pm) return;
        float zd = Vector3.Dot(GetDirection(), pm.cam.transform.forward) > 0 ? 1.0f : -1.0f;
        pm.SetCurrentZipline(this, zd);
    }

    public Vector3 GetOnZiplinePos(Vector3 pos, float offset){
        return Vector3.Project(pos-endA.transform.position, GetDirection()) + endA.transform.position + Vector3.up*offset;
    }

    public bool IsOnZipline(Vector3 pos){
        var zPos = GetOnZiplinePos(pos, 0);
        return Vector3.Dot(zPos - endA.transform.position, zPos - endB.transform.position) < 0;
    }

    public void toggleColor(bool on){

        var rend = GetComponentInChildren<Renderer>();
        if (on) rend.material.EnableKeyword("_EMISSION");
        else rend.material.DisableKeyword("_EMISSION");
    }

    public void OnTriggerEnter(Collider other)
    {
        toggleColor(true);
    }

    private void OnTriggerExit(Collider other)
    {
        toggleColor(false);
    }
}
