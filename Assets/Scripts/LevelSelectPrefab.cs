using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        if (!PlayerPrefs.HasKey ("LastLevelCompleted")) PlayerPrefs.SetInt ("LastLevelCompleted", 0);
        if (thisLevel > PlayerPrefs.GetInt ("LastLevelCompleted") + 1) DisableLevel ();
    }

    void DisableLevel () {
        play.interactable = false;
        logo.color = greyOutColour;
    }

    private void Start () {
        LoadMiniLeaderboard ();
    }

    private void LoadMiniLeaderboard () {

    }

    public void GoToLevel () => SceneManager.LoadScene (thisLevel);
}
