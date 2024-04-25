using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BounceScrollUI : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;
    
    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;
    [SerializeField] Scrollbar scrollMask;
    private float vel;
    [SerializeField] float hiddenValue;
    [SerializeField] float visibleValue;

    private float currentValue;
    private float targetValue;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
        currentValue = hiddenValue;
        targetValue = hiddenValue;
    }

    private void OnEnable()
    {
        ActionEvents.StartedGameplayCutscene += BounceIn;
    }

    private void OnDisable()
    {
        ActionEvents.StartedGameplayCutscene -= BounceIn;
    }
    
    private async void BounceIn()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        targetValue = visibleValue;
        await Task.Delay(TimeSpan.FromSeconds(5));
        targetValue = hiddenValue;
    }

    private void Update()
    {
        scrollMask.value = currentValue;
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.unscaledDeltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentValue, ref vel, targetValue, springParams);
    }
}
