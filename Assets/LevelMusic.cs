using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioClip music;

    [Range(0f, 1f)]
    public float volume = 1;

    void Start()
    {
        MusicManager.singleton.SetCurrentMusic(music, volume);
    }
}
