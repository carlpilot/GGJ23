using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject loseMenu;
    public GameObject winMenu;
    public Transform leaderboardContainer;
    public GameObject leaderboardItemPrefab;
    public TMP_Text usernameText;
    public TMP_Text thisTime;
    public TMP_Text bestTime;
    public TMP_Text rank;

    [Header ("Curtains")]
    public Color loseCurtainColour;
    public Color winCurtainColour;
    public Color startCurtainColour;

    public bool isPaused { get; private set; }
    PlayerMovement player;
    bool movement;

    Curtains curtains;

    private void Awake () {
        Time.timeScale = 1;
        player = FindObjectOfType<PlayerMovement> ();
        curtains = FindObjectOfType<Curtains> ();
    }

    private void Start () {
        curtains.SetPosition (curtains.closeAmount);
        curtains.SetColour (startCurtainColour);
        curtains.Open ();
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (!isPaused) Pause (); else Unpause ();
        }
    }

    public void Pause () {
        movement = player.isMovementEnabled;
        player.SetMovementEnabled (false);
        pauseMenu.SetActive (true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Unpause () {
        player.SetMovementEnabled (movement);
        pauseMenu.SetActive (false);        
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Win () {

    }

    public void Lose () {
        loseMenu.SetActive (true);
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
