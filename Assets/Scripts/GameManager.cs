using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
    public bool isPaused { get; private set; }
    CursorLockMode clm;
    PlayerMovement player;
    bool movement;

    [Header ("Win/Lose")]
    public Color loseCurtainColour;

    Curtains curtains;

    private void Awake () {
        Time.timeScale = 1;
        player = FindObjectOfType<PlayerMovement> ();
        curtains = FindObjectOfType<Curtains> ();
    }

    private void Start () {
        curtains.SetPosition (curtains.closeAmount);
        curtains.SetColour (loseCurtainColour);
        curtains.Open ();
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (!isPaused) Pause (); else Unpause ();
        }
    }

    public void Pause () {
        //clm = Cursor.lockState;
        //Cursor.lockState = CursorLockMode.None;
        movement = player.isMovementEnabled;
        player.SetMovementEnabled (false);
        pauseMenu.SetActive (true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Unpause () {
        //Cursor.lockState = clm;
        player.SetMovementEnabled (movement);
        pauseMenu.SetActive (false);        
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Win () {

    }

    public void Lose () {
        curtains.SetColour (loseCurtainColour);
        curtains.Close ();
    }

    public void NextLevel () {
        SwitchScene (SceneManager.GetActiveScene ().buildIndex + 1);
    }

    public void RestartLevel () {
        SwitchScene (SceneManager.GetActiveScene ().buildIndex);
    }

    public void SwitchScene (int scene) {
        SceneManager.LoadScene (scene);
    }
}
