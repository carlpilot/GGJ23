using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

    public float speed = 0.2f;
    public float cutoff = 2300;
    private bool started = false;

    GUISlideSelect gss;
    Vector3 startPosition;

    private void Awake () {
        gss = FindObjectOfType<GUISlideSelect> ();
        startPosition = transform.position;
    }

    void Update () {
        if (started) {
            transform.Translate (0, speed * Time.unscaledDeltaTime * Screen.height, 0);
            if (transform.position.y >= cutoff / 1920.0f * Screen.height) BackToMenu ();
        }
    }

    public void BackToMenu () {
        gss.Transition (0);
        started = false;
    }

    public void StartCredits () {
        transform.position = startPosition;
        started = true;
    }
}