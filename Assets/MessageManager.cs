using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager m;
    public TMP_Text text;
    float msgTimer = 0;

    void Awake()
    {
        m = this;
    }
    
    void Update()
    {
        if (msgTimer < 0){
            text.gameObject.SetActive(false);
        }
    }

    public void SetMessageWithTimeout(string msg, float timeout){
        text.gameObject.SetActive(true);
        msgTimer = timeout;
        text.text = msg;
    }

    public void ClearMessage(){
        msgTimer = -1;
    }
}
