using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerParticleManager : MonoBehaviour
{
    private PlayerBase player;
    private Rigidbody rb;
    public ParticleSystem[] jetStreams;
    public ParticleSystem playerSpeedLines;
    public ParticleSystem playerGrindSparks;

    private void Start()
    {
        player = FindObjectOfType<PlayerBase>();
        rb = player.rb;
    }
    
    public void JetStreamActive(bool isEnabled)
    {
        foreach (var jetStream in jetStreams)
        {
            if (isEnabled) jetStream.Play();
            else jetStream.Stop();
        }
    }

    private void Update()
    {
        UpdateJetStream();
    }

    private void UpdateJetStream()
    {
        if (!jetStreams[0].isEmitting) return;
        foreach (var jetStream in jetStreams)
        {
            var velOverLife = jetStream.velocityOverLifetime;
            float t = Mathf.InverseLerp(60, 100, rb.velocity.magnitude);
            velOverLife.zMultiplier = Mathf.Lerp(0, 5, t);
        }
    }
}
