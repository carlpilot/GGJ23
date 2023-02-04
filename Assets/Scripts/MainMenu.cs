using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour {

    public TMP_InputField usernameInput;
    public TMP_Text usernameConfirm;

    private void Start () {
        usernameConfirm.text = "Your username is " + PlayerPrefs.GetString ("Username");
    }

    public void SaveUsername () {
        PlayerPrefs.SetString ("Username", usernameInput.text);
        usernameConfirm.text = "Your username is " + PlayerPrefs.GetString ("Username");
    }

    public void SwitchScene (int scene) {
        SceneManager.LoadScene (scene);
    }
}
