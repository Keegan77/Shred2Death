using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMagnitude : MonoBehaviour
{
    //this script sets the volume of it's audio source based on the magnitude of the player's velocity
    [SerializeField] Rigidbody rb;
    private AudioSource audioSource;
    [SerializeField] private PlayerBase player;
    [SerializeField] private bool lockSkateState;
    [SerializeField] private bool lockAirState;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (lockSkateState)
        {
            if (player.stateMachine.currentState != player.skatingState && 
                player.stateMachine.currentState != player.driftState)
            {
                audioSource.volume = 0;
                return;
            }
        }
        if (lockAirState)
        {
            if (player.stateMachine.currentState != player.airborneState &&
                player.stateMachine.currentState != player.nosediveState)
            {
                audioSource.volume = 0;
                return;
            }
        }

        audioSource.volume = Mathf.Clamp01(rb.velocity.magnitude / 90);
    }
}
