using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager m;
    public TMP_Text hintText;
    public TMP_Text quoteText;
    float hintTimer = 0;
    float quoteTimer = 0;

    public Animator hintAnimator;

    char[] remainingQuoteText;

    void Awake()
    {
        m = this;
    }
    
    void Update()
    {
        hintTimer -= Time.deltaTime;
        quoteTimer -= Time.deltaTime;

        if (hintTimer < 0){
            hintAnimator.SetBool("isShown", false);
            hintText.gameObject.SetActive(false);
        }
        if (quoteTimer < 0){
            if (remainingQuoteText.Length == 0){
                quoteText.text = "";
            } else if (remainingQuoteText.Length == 1){
                quoteText.text += remainingQuoteText[0];
                remainingQuoteText = new char[0];
                quoteTimer = 5f;
            } else{
                quoteText.text += remainingQuoteText[0];
                //remainingQuoteText = remainingQuoteText[1:];
                var temp = new List<char>(remainingQuoteText);
                temp.RemoveAt(0);
                remainingQuoteText = temp.ToArray();
                quoteTimer = 0.025f;
            }
        }
    }

    public void SetHintWithTimeout(string msg, float timeout){
        hintText.gameObject.SetActive(true);
        hintTimer = timeout;
        hintText.text = msg;
        hintAnimator.SetBool("isShown", true);
    }

    public void ClearHint(){
        hintTimer = -1;
        hintAnimator.SetBool("isShown", false);
    }

    public void SetQuote(string msg){
        quoteText.gameObject.SetActive(true);
        quoteText.text = "";
        // Creating array of string length 
        char[] ch = new char[msg.Length];
        // Copy character by character into array 
        for (int i = 0; i < msg.Length; i++) {
            ch[i] = msg[i];
        }
        remainingQuoteText = ch;
    }

    public void ClearQuote(){
        quoteText.text = "";

    }
}
