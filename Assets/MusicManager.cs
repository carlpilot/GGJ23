using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;
    public bool restartMusicOnLevelReload = false;
    void Awake()
    {
        transform.parent = null;
        if (singleton == null){
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            print("Registered Music Manager");
        } else{
            Destroy(gameObject);
        }
    }

    AudioClip currentMusic;
    
    public void SetCurrentMusic(AudioClip clip, float volume){
        if (currentMusic != clip || restartMusicOnLevelReload){
            currentMusic = clip;
            var AS = GetComponent<AudioSource>();
            AS.clip = clip;
            AS.volume = volume;
            AS.Stop();
            AS.Play();
        }
    }
}
