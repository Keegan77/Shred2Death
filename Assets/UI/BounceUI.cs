using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines.Primitives;
using UnityEngine;

public class BounceUI : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;

    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;
    
    [SerializeField] float targetXPosition;
    private float currentXPosition;
    private float vel;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
    }

    private void Start()
    {
        currentXPosition = transform.position.x;
    }
    
    private void Update()
    {
        transform.position = new Vector3(currentXPosition, transform.position.y, transform.position.z);
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.deltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentXPosition, ref vel, targetXPosition, springParams);
    }
}
