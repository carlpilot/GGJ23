using UnityEngine;
using TMPro;

public class LeaderItem : MonoBehaviour {

    public TMP_Text rank, username, time;

    public void Setup (int _rank, string _username, float _time) {
        rank.text = _rank.ToString ();
        username.text = _username;
        time.text = Timer.TimeFormat (_time);
    }
}