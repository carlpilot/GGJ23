using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    TMPro.TMP_Text timerText;

    bool counting = false;

    float count = 0;
    float startTime = 0;

    private void Update () {
        if(counting) count = Time.timeSinceLevelLoad - startTime;
        if (timerText != null) timerText.text = TimeFormat (count);
    }

    public void StartTime () {
        counting = true;
        startTime = Time.timeSinceLevelLoad;
    }

    public void StopTime() => counting = false;

    public bool isCounting { get => counting; }

    public float time { get => count; }

    public static string TimeFormat (float t) {
        int minutes = Mathf.FloorToInt (t / 60.0f);
        int seconds = Mathf.FloorToInt (t - 60.0f * minutes);
        int hundredths = Mathf.FloorToInt ((t - 60.0f * minutes - (float) seconds) * 100f);
        return string.Format ("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, hundredths);
    }
}
