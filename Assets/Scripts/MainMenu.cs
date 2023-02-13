using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour {

    [Header("Username")]
    public TMP_InputField usernameInput;
    public TMP_Text usernameConfirm;
    public Button[] buttonsToDisableWithoutUsername;

    [Header("Level Select")]
    public GameObject levelSelectPrefab;
    public Transform levelScrollView;
    public Sprite[] levelSprites;

    [Header ("Options")]
    public AudioMixer mixer;
    public Slider soundVol;
    public Slider musicVol;
    public TMP_Text soundVolPct, musicVolPct;
    public Toggle hintsToggle, dialogueToggle;

    private void Start () {
        Time.timeScale = 1;
        if (PlayerPrefs.HasKey ("Username") && isUsernameValid) {
            string existingUsername = PlayerPrefs.GetString ("Username");
            usernameConfirm.text = "Hello, " + existingUsername + "!";
            usernameInput.textComponent.color = Color.white;
            usernameInput.text = existingUsername;
        } else {
            usernameInput.textComponent.color = Color.red;
            usernameConfirm.text = "Please enter a username";
        }
        usernameInput.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;

        LoadLevels ();

        if(PlayerPrefs.HasKey("MusicVolume") && PlayerPrefs.HasKey("SoundVolume")) {
            Debug.Log ("Found volume preferences: Sound=" + PlayerPrefs.GetFloat ("SoundVolume") + " Music=" + PlayerPrefs.GetFloat ("MusicVolume"));
            soundVol.value = PlayerPrefs.GetFloat ("SoundVolume");
            musicVol.value = PlayerPrefs.GetFloat ("MusicVolume");
            UpdateSoundVolume ();
            UpdateMusicVolume ();
        }
        if (PlayerPrefs.HasKey ("ShowHints")) hintsToggle.isOn = PlayerPrefs.GetInt ("ShowHints") == 1;
        if (PlayerPrefs.HasKey ("ShowDialogue")) dialogueToggle.isOn = PlayerPrefs.GetInt ("ShowDialogue") == 1;
    }

    private void Update () {
        bool valid = isEnteredUsernameValid;
        foreach (Button b in buttonsToDisableWithoutUsername) {
            b.interactable = valid && !GetComponent<VersionChecker>().isPromptOpen;
        }
    }

    void LoadLevels () {
        for(int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
            GameObject g = Instantiate (levelSelectPrefab, levelScrollView);
            g.GetComponent<LevelSelectPrefab> ().Setup (i, levelSprites[i - 1]);
        }
    }

    public void SaveUsername () {
        string username = usernameInput.text;
        if (ValidateUsername (username) == "") {
            usernameInput.textComponent.color = Color.white;
            PlayerPrefs.SetString ("Username", username);
            usernameConfirm.text = "Hello,  " + PlayerPrefs.GetString ("Username") + "!";
        } else {
            usernameConfirm.text = ValidateUsername (username);
            usernameInput.textComponent.color = Color.red;
        }
    }

    public string ValidateUsername (string username) {
        //return username.Length >= 3 && username.Length <= 16;
        if (username.Length < 3) return "Your username must be at least 3 characters";
        if (username.Length > 16) return "Your username must be at most 16 characters";
        return "";
    }

    public bool isUsernameValid { get => ValidateUsername (PlayerPrefs.GetString ("Username")) == ""; }
    public bool isEnteredUsernameValid { get => ValidateUsername (usernameInput.text) == ""; }

    public void UpdateHintsPreference () => PlayerPrefs.SetInt ("ShowHints", hintsToggle.isOn ? 1 : 0);
    public void UpdateDialoguePreference () => PlayerPrefs.SetInt ("ShowDialogue", dialogueToggle.isOn ? 1 : 0);
    public void UpdateSoundVolume () {
        soundVolPct.text = Mathf.RoundToInt (soundVol.value / soundVol.maxValue * 100.0f) + "%";
        float sv = convertSliderTo_dB (soundVol.value / soundVol.maxValue);
        mixer.SetFloat ("SoundVolume", sv);
        PlayerPrefs.SetFloat ("SoundVolume", soundVol.value);
    }
    public void UpdateMusicVolume () {
        musicVolPct.text = Mathf.RoundToInt (musicVol.value / musicVol.maxValue * 100.0f) + "%";
        float mv = convertSliderTo_dB (musicVol.value / musicVol.maxValue);
        mixer.SetFloat ("MusicVolume", mv);
        PlayerPrefs.SetFloat ("MusicVolume", musicVol.value);
    }

    // slider 0-1, convert to logarithmic scale from -80 dB to 20 dB where 0.5 is 0 dB
    public float convertSliderTo_dB (float sliderValue) {
        return 70.1734f * Mathf.Log10 (25.6098f * sliderValue + 1) - 80;
    }

    public void SwitchScene (int scene) => SceneManager.LoadScene (scene);

    public void Quit () => Application.Quit ();
}
