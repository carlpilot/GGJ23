using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectPrefab : MonoBehaviour {

    public TMP_Text title;
    public Image logo;
    public LeaderItem[] miniLeaderboard;
    public Button play;
    public Color greyOutColour;

    int thisLevel;

    public void Setup (int level, Sprite logoSprite) {
        thisLevel = level;
        title.text = "Level " + thisLevel;
        logo.sprite = logoSprite;
        logo.GetComponent<AspectRatioFitter> ().aspectRatio = logo.sprite.rect.width / logo.sprite.rect.height;

        if (!PlayerPrefs.HasKey ("LastLevelCompleted")) PlayerPrefs.SetInt ("LastLevelCompleted", 0);
        if (thisLevel > PlayerPrefs.GetInt ("LastLevelCompleted") + 1) DisableLevel ();
    }

    void DisableLevel () {
        play.interactable = false;
        logo.color = greyOutColour;
    }

    private void Start () {
        GetHighScores ();
    }

    private void LoadMiniLeaderboard (string highscores) {
        string[] scores = highscores.Split ('\n');
        string username = PlayerPrefs.GetString ("Username");
        for (int i = 0; i < Mathf.Min(scores.Length, miniLeaderboard.Length); i++) {
            if (scores[i].Length < 2) continue;
            string[] vals = scores[i].Split ('|');
            string casedUsername = vals[3] != "" ? vals[3] : vals[0]; // use cased username if present
            miniLeaderboard[i].Setup (i + 1, casedUsername, int.Parse (vals[2]) / 1000.0f);
        }
    }

    public void GetHighScores () {
        StartCoroutine (GetHSHelper ("http://dreamlo.com/lb/" + SecretCode.Public (thisLevel) + "/pipe-seconds-asc"));
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
                    LoadMiniLeaderboard (highscores);
                    break;
            }
        }
    }

    public void GoToLevel () => SceneManager.LoadScene (thisLevel);
}
