using UnityEngine;

[RequireComponent (typeof (Animator))]
public class AnimRandomSpeed : MonoBehaviour {

    public float minValue = 0.9f;
    public float maxValue = 1.1f;

    private void Start () {
        GetComponent<Animator> ().speed = Mathf.Lerp (minValue, maxValue, Random.value);
    }
}
