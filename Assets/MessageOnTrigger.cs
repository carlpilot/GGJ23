using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOnTrigger : MonoBehaviour
{
    public bool clear;
    public string msg;
    public float timeout = 10f;

    bool triggered = false;
    
    void OnTriggerEnter(Collider col){
        if (triggered) return;
        triggered = true;
        if(!clear) MessageManager.m.SetMessageWithTimeout(msg, timeout);
        else MessageManager.m.ClearMessage();
    }
}
