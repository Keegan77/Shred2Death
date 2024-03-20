using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSFXLoop : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    
    private void PlayLoopAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
    
    private void StopLoopAudio()
    {
        audioSource.Stop();
    }
    
    private void OnEnable()
    {
        ActionEvents.PlayLoopAudio += PlayLoopAudio;
        ActionEvents.StopLoopAudio += StopLoopAudio;
    }

    private void OnDisable()
    {
        ActionEvents.PlayLoopAudio -= PlayLoopAudio;
        ActionEvents.StopLoopAudio -= StopLoopAudio;
    }

}
