using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    private void Update () {
        bool valid = isEnteredUsernameValid;
        foreach (Button b in buttonsToDisableWithoutUsername) {
            b.interactable = valid;
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

    public void SwitchScene (int scene) => SceneManager.LoadScene (scene);

    public void Quit () => Application.Quit ();
}
