using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
    public bool isPaused { get; private set; }
    CursorLockMode clm;

    private void Awake () {
        Time.timeScale = 1;
    }

    private void Start () {
        
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (!isPaused) Pause (); else Unpause ();
        }
    }

    public void Pause () {
        clm = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive (true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Unpause () {
        Cursor.lockState = clm;
        pauseMenu.SetActive (false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Win () {

    }

    public void Lose () {

    }

    public void NextLevel () {

    }

    public void RestartLevel () {
        SwitchScene (SceneManager.GetActiveScene ().buildIndex);
    }

    public void SwitchScene (int scene) {
        SceneManager.LoadScene (scene);
    }
}
