using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines.Primitives;
using UnityEngine;

public class BounceUI : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;

    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;
    
    public float targetXPosition;
    public float startingXPosition;
    public float endingXPosition;
    private float currentXPosition;
    private float vel;

    public bool isMask;
    [SerializeField] bool useCurrentAsStarting = true;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
    }

    private void Start()
    {
        if (useCurrentAsStarting)
        {
            targetXPosition = transform.position.x;
            startingXPosition = transform.position.x;
            return;
        }
        targetXPosition = startingXPosition;
        currentXPosition = startingXPosition;
    }

    private void OnEnable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene += BounceIn;
    }

    private void OnDisable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene -= BounceIn;
    }
    
    private async void BounceIn()
    {
        targetXPosition = endingXPosition;
        await Task.Delay(TimeSpan.FromSeconds(7));
        targetXPosition = startingXPosition;
    }

    private void Update()
    {
        transform.position = new Vector3(currentXPosition, transform.position.y, transform.position.z);
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.unscaledDeltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentXPosition, ref vel, targetXPosition, springParams);
    }
}
