using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour {

    public TMP_InputField usernameInput;
    public TMP_Text usernameConfirm;

    public void SaveUsername () {
        PlayerPrefs.SetString ("Username", usernameInput.text);
        usernameConfirm.text = "Your username is " + PlayerPrefs.GetString ("Username");
    }
}
