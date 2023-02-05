using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    GameManager gm;

    private void Awake () {
        gm = FindObjectOfType<GameManager> ();
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.layer == 6) gm.Lose ();
    }
}
