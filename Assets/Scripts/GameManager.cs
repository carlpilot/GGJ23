using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    [Header ("Menus")]
    public GameObject pauseMenu;
    public GameObject loseMenu;
    public GameObject winMenu;
    public Transform leaderboardContainer;
    public GameObject leaderboardItemPrefab;
    public TMP_Text usernameText;
    public TMP_Text thisTime;
    public TMP_Text bestTime;
    public TMP_Text rank;
    public GameObject loadingScoresText;

    [Header ("Win / Lose Logic")]
    public float loseWait = 2.0f;

    [Header ("Curtains")]
    public Color loseCurtainColour;
    public Color winCurtainColour;
    public Color startCurtainColour;

    public bool isPaused { get; private set; }
    PlayerMovement player;
    bool movement;

    public bool winnable { get; private set; } = true;

    Timer timer;
    Curtains curtains;

    public int level { get => SceneManager.GetActiveScene ().buildIndex; }

    private void Awake () {
        Time.timeScale = 1;
        player = FindObjectOfType<PlayerMovement> ();
        timer = FindObjectOfType<Timer> ();
        curtains = FindObjectOfType<Curtains> ();
    }

    private void Start () {
        curtains.SetPosition (curtains.closeAmount);
        curtains.SetColour (startCurtainColour);
        curtains.Open ();
    }

    private void Update () {
        // start timer on WASD or space
        if (!timer.isCounting && winnable && (Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0 || Input.GetKey (KeyCode.Space))) timer.StartTime ();

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
        if (!winnable) return;
        winnable = false;
        winMenu.SetActive (true);
        curtains.SetColour (winCurtainColour);
        curtains.Close ();
        PutGetHighScores (timer.time);
        timer.StopTime ();
        player.SetMovementEnabled (false);
    }

    public void Lose () {
        winnable = false;
        loseMenu.SetActive (true);
        curtains.SetColour (loseCurtainColour);
        curtains.Close ();
        timer.StopTime ();
        player.SetMovementEnabled (false);
        StartCoroutine (WaitResetLevel ());
    }

    IEnumerator WaitResetLevel () {
        AsyncOperation a = SceneManager.LoadSceneAsync (level);
        a.allowSceneActivation = false;
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < loseWait) {
            yield return null;
        }
        //RestartLevel ();
        a.allowSceneActivation = true;
    }

    public void NextLevel () {
        SwitchScene (SceneManager.GetActiveScene ().buildIndex + 1);
    }

    public void RestartLevel () {
        SwitchScene (level);
    }

    public void SwitchScene (int scene) {
        SceneManager.LoadScene (scene);
    }

    public void GetHighScores () {
        StartCoroutine (GetHSHelper ("http://dreamlo.com/lb/" + SecretCode.Public (level) + "/pipe-seconds-asc"));
    }

    public void PutGetHighScores (float time) {
        int timeMS = Mathf.FloorToInt (time * 1000);
        int score = 10000000 - timeMS; // lower time = higher score
        StartCoroutine (GetHSHelper ("http://dreamlo.com/lb/" + SecretCode.Private (level) + "/add-pipe-seconds-asc/" + PlayerPrefs.GetString ("Username") + "/" + score + "/" + timeMS));
    }

    IEnumerator GetHSHelper (string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get (uri)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest ();

            string[] pages = uri.Split ('/');
            int page = pages.Length - 1;

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError (pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError (pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var highscores = webRequest.downloadHandler.text;
                    LoadLeaderboard (highscores);
                    break;
            }
        }
    }

    public void LoadLeaderboard (string highscores) {
        string[] scores = highscores.Split ('\n');
        string u = PlayerPrefs.GetString ("Username");
        rank.text = "#";
        bestTime.text = "-";
        for (int i = 0; i < scores.Length; i++) {
            if (scores[i].Length < 2) continue;
            string[] vals = scores[i].Split ('|');
            GameObject newLBI = Instantiate (leaderboardItemPrefab);
            newLBI.transform.SetParent (leaderboardContainer, false);
            LeaderItem lbi = newLBI.GetComponent<LeaderItem> ();
            lbi.Setup (i + 1, vals[0], int.Parse (vals[2]) / 1000.0f);
            if (vals[0] == u) {
                rank.text = (i + 1).ToString ();
                bestTime.text = Timer.TimeFormat (int.Parse (vals[2]) / 1000.0f);
            }
        }
        usernameText.text = PlayerPrefs.GetString ("Username");
        thisTime.text = Timer.TimeFormat (timer.time);
        loadingScoresText.SetActive (false);
    }
}
