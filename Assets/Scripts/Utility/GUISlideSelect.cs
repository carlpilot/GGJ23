using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISlideSelect : MonoBehaviour
{
    public RectTransform[] slides;
    public GameObject clickThroughBlocker;

    public Vector3[] cameraPositions;
    public Vector3[] cameraRotations;

    public float speed = 1f;

    public AnimationCurve speedMultiplierByPosition;
    public AnimationCurve cameraTransitionCurve;

    int originSlide = 0;

    public int CurrentSlide { get; private set; } = 0;
    public float slideDist { get => slides[0].rect.width; }

    private void Start () {
        
        // Scale screens to Screen
        for(int i = 1; i < slides.Length; i++) {
            //slides[i].SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, Screen.width);
            slides[i].SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, slides[0].rect.width);
        }
        
    }

    public void Transition (int slide) {
        originSlide = CurrentSlide;
        CurrentSlide = slide;
        StartCoroutine ("transitionSlide");
    }

    IEnumerator transitionSlide () {
        clickThroughBlocker.SetActive (true);
        float distTravelled = 0f;
        bool right = (CurrentSlide != 0); // sliding direction: True = right, false = left

        // make right slides active
        for (int i = 0; i < slides.Length; i++) {
            slides[i].gameObject.SetActive (i == 0 || i == CurrentSlide || i == originSlide);
        }

        // set up start positions
        Vector2[] startPositions = new Vector2[slides.Length];
        for(int i = 0; i < slides.Length; i++) {
            startPositions[i] = slides[i].anchoredPosition;
        }

        Debug.Log ("Transitioning to " + CurrentSlide);

        // animate
        while (distTravelled < slideDist) {
            float d = (right ? -1f : 1f) * speed / Time.fixedDeltaTime * speedMultiplierByPosition.Evaluate (distTravelled / slideDist);
            foreach (RectTransform r in slides) {
                r.anchoredPosition += Vector2.right * d;
            }
            distTravelled += Mathf.Abs (d);
            yield return new WaitForEndOfFrame ();

            float cameraInterp = cameraTransitionCurve.Evaluate (distTravelled / slideDist);
            Camera.main.transform.position = Vector3.Lerp (cameraPositions[originSlide], cameraPositions[CurrentSlide], cameraInterp);
            Camera.main.transform.rotation = Quaternion.Slerp (Quaternion.Euler (cameraRotations[originSlide]), Quaternion.Euler (cameraRotations[CurrentSlide]), cameraInterp);
        }

        // lock final positions (eliminates frame gaps)
        for (int i = 0; i < slides.Length; i++) {
            slides[i].anchoredPosition = startPositions[i] + Vector2.right * (right ? -1f : 1f) * slideDist;
        }

        clickThroughBlocker.SetActive (false);
    }
}
