using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Curtains : MonoBehaviour {

    public RectTransform left, right;
    Image l_img, r_img;

    public float speed = 1000;
    public float screenCoverFraction = 0.5f;

    public float closeAmount { get; private set; }

    float leftStartX, rightStartX;

    private void Awake () {
        l_img = left.GetComponent<Image> ();
        r_img = right.GetComponent<Image> ();
        leftStartX = left.position.x;
        rightStartX = right.position.x;
        closeAmount = screenCoverFraction * Screen.width;
    }

    public void SetColour (Color c) {
        l_img.color = c;
        r_img.color = c;
    }

    public void Close () {
        left.gameObject.SetActive (true);
        right.gameObject.SetActive (true);
        StartCoroutine (MoveCurtains (closeAmount));
    }

    public void Open () {
        left.gameObject.SetActive (true);
        right.gameObject.SetActive (true);
        StartCoroutine (MoveCurtains (0));
    }

    public void SetPosition (float target) {
        left.position = Vector3.right * (leftStartX + target) + Vector3.Scale(left.position, Vector3.up + Vector3.forward);
        right.position = Vector3.right * (rightStartX - target) + Vector3.Scale (right.position, Vector3.up + Vector3.forward); ;
    }

    IEnumerator MoveCurtains (float target) {
        float targetL = leftStartX + target;
        float targetR = rightStartX - target;

        float frameMovement = speed * Time.unscaledDeltaTime;

        while(true) {
            float ltrans = Mathf.Clamp (targetL - left.position.x, -frameMovement, frameMovement);
            float rtrans = Mathf.Clamp (targetR - right.position.x, -frameMovement, frameMovement);
            left.Translate (Vector3.right * ltrans);
            right.Translate (Vector3.right * rtrans);
            yield return new WaitForEndOfFrame ();

            if (ltrans == 0 && rtrans == 0) break;
        }

        if(target == 0) {
            left.gameObject.SetActive (false);
            right.gameObject.SetActive (false);
        }
    }
}
