using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public float speed = 0.002f;
    public float cutoff = 4000;


    void Update () {
        
        transform.Translate(0, speed, 0);
        
        if(transform.position.y >= cutoff){
            SceneManager.LoadScene (0);
        }
    
    }

    public void BackToMenu () => SceneManager.LoadScene (0);
}
