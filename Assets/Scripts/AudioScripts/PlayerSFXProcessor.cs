using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXProcessor : MonoBehaviour
{

    private void OnEnable()
    {
        ActionEvents.PlayerSFXOneShot += PlayOneShotSound;
    }

    private void OnDisable()
    {
        ActionEvents.PlayerSFXOneShot -= PlayOneShotSound;
    }

    private void PlayOneShotSound(AudioClip audioClip, float cutoffTime)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.pitch = Time.timeScale;
        audioSource.volume = PlayerPrefs.GetFloat(SettingsManager.PrefNames.SFXVolume);
        audioSource.clip = audioClip;
        audioSource.time = cutoffTime;
        audioSource.Play();
        StartCoroutine(WaitForSoundToFinish(audioSource));
    }
    
    private IEnumerator WaitForSoundToFinish(AudioSource audioSource)
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        Destroy(audioSource);
    }
    
}
