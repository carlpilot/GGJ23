using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOnTrigger : MonoBehaviour
{
    public bool clear;
    public string msg;
    public float timeout = 10f;

    public bool isHint;

    bool triggered = false;
    
    void OnTriggerEnter(Collider col){
        if (triggered) return;
        triggered = true;
        if (isHint){
            if(!clear) MessageManager.m.SetHintWithTimeout(msg, timeout);
            else MessageManager.m.ClearHint();
        } else{
            if(!clear) MessageManager.m.SetQuote(msg);
            else MessageManager.m.ClearQuote();
        }
    }
}
