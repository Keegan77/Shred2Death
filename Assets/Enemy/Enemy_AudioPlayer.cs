using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as an interface between 
/// </summary>
public class Enemy_AudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    /// <summary>
    /// Picks one audioclip out of a specified array and plays it on the audio player.
    /// </summary>
    public void playClipRandom (AudioClip[] clips)
    {
        int i = Random.Range (0, clips.Length);
        audioSource.PlayOneShot (clips[i]);
    }

}
