using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedLinesController : MonoBehaviour
{
    private PlayerBase player;
    Rigidbody rb;
    [SerializeField] private ParticleSystem speedLines;
    [SerializeField] AnimationCurve speedLinesRateOverTime;

    private void Start()
    {
        player = FindObjectOfType<PlayerBase>();
        rb = player.rb;
    }

    private void Update()
    {
        var emission = speedLines.emission;
        float t = Mathf.InverseLerp(50, 100, rb.velocity.magnitude);
        emission.rateOverTime = Mathf.Lerp(0, 40, speedLinesRateOverTime.Evaluate(t));
    }
}
