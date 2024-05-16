using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMusicVolume : MonoBehaviour
{
    private AudioSource music;


    private void Start()
    {
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        music.volume = PlayerPrefs.GetFloat(SettingsManager.PrefNames.MusicVolume);
    }
}
