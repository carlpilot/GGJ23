using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour {

    public TMP_InputField usernameInput;
    public TMP_Text usernameConfirm;

    private void Start () {
        if (PlayerPrefs.HasKey ("Username")) {
            string existingUsername = PlayerPrefs.GetString ("Username");
            usernameConfirm.text = "Hello, " + existingUsername + "!";
            usernameInput.text = existingUsername;
        }
        usernameInput.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
    }

    public void SaveUsername () {
        string username = usernameInput.text;
        if (ValidateUsername (username) == "") {
            usernameInput.textComponent.color = Color.black;
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

    public void SwitchScene (int scene) {
        SceneManager.LoadScene (scene);
    }
}
