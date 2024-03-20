using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXContainerSingleton : MonoBehaviour
{
    public static SFXContainerSingleton Instance { get; private set; }
    
    [Header("Player SFX")]
    public List<AudioClip> ollieSounds;
    public List<AudioClip> landingSounds;
    public List<AudioClip> kickOffSounds;
    public AudioClip popShuvItSound;
    public List<AudioClip> grindImpactNoises;
    public List<AudioClip> grindSounds;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
