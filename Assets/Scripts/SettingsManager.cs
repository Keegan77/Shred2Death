using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // using multiple methods for the same thing is dumb but i'm being rushed so im doing it anyways
    // im sorry for this war crime
    // the reason im doing this is because unity events on UI elements can't take in multiple parameters
    // as a limitation of the abstraction. right now we don't have many settings, so this is like,
    // fine i guess for now, just annoying. can't even use a struct to pass in multiple because those can't be 
    // serialized. unity moment
    
    // again this code is probably the worst thing i've written in my entire life but this shit is due today so
    // dont blame me
    //todo: create a more advanced settings management system where you can add more settings in a scalable manner. this script sucks

    [SerializeField] private Slider sensitivitySlider, musicSlider, sfxSlider;
    private void Start()
    {
        //check if any player pref is 0 and set it to the default value
        if (PlayerPrefs.GetFloat(PrefNames.Sensitivity) == 0)
        {
            PlayerPrefs.SetFloat(PrefNames.Sensitivity, 100);
        }
        if (PlayerPrefs.GetFloat(PrefNames.MusicVolume) == 0)
        {
            PlayerPrefs.SetFloat(PrefNames.MusicVolume, .5f);
        }
        if (PlayerPrefs.GetFloat(PrefNames.SFXVolume) == 0)
        {
            PlayerPrefs.SetFloat(PrefNames.SFXVolume, 1f);
        }
        
        //set the sliders to the player prefs. i hate this method of doing things
        sensitivitySlider.value = Mathf.InverseLerp(25, 300, PlayerPrefs.GetFloat(PrefNames.Sensitivity));
        musicSlider.value = Mathf.InverseLerp(0, 1, PlayerPrefs.GetFloat(PrefNames.MusicVolume));
        sfxSlider.value = Mathf.InverseLerp(0, 1, PlayerPrefs.GetFloat(PrefNames.SFXVolume));
    }

    public static class PrefNames
    {
        public const string Sensitivity = "Sensitivity";
        public const string MusicVolume = "MusicVolume";
        public const string SFXVolume = "SFXVolume";
    }
    
    public void SaveSensitivity(Slider sliderObj)
    {
        float newSens = Mathf.Lerp(25, 300, sliderObj.value);
        PlayerPrefs.SetFloat(PrefNames.Sensitivity, newSens);
        Debug.Log("Sensitivity saved as " + newSens);
    }
    
    public void SaveMusicVolume(Slider sliderObj)
    {
        float newVol = Mathf.Lerp(0, 1, sliderObj.value);
        PlayerPrefs.SetFloat(PrefNames.MusicVolume, newVol);
        Debug.Log("MusicVolume saved as " + newVol);
    }
    
    public void SaveSFXVolume(Slider sliderObj)
    {
        float newVol = Mathf.Lerp(0, 1, sliderObj.value);
        PlayerPrefs.SetFloat(PrefNames.SFXVolume, newVol);
        Debug.Log("SFXVolume saved as " + newVol);
    } //this is the worst
}
