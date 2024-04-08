using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJetStreamController : MonoBehaviour
{
    private PlayerBase player;
    Rigidbody rb;
    [SerializeField] private ParticleSystem jetStream;

    private void Start()
    {
        player = FindObjectOfType<PlayerBase>();
        rb = player.rb;
    }

    private void Update()
    {
        float t = Mathf.InverseLerp(50, 100, rb.velocity.magnitude);
        //jetStream.duration = Mathf.Lerp(0, 40, speedLinesRateOverTime.Evaluate(t));
    }
}
