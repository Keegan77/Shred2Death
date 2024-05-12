using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines.Primitives;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class BounceUI : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;
    
    private Vector2 referenceResolution = new Vector2(1920, 1080);

    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;
    
    private Vector3 homePosition;
    [SerializeField] Vector3 targetLocalDisplacement;
    private Vector3 currentSpringPosition;
    private Vector3 targetPosition; //used as the current position to spring towards
    
    private Vector3 endingSpringPosition; //used to hold the end position
    private Vector3 vel;

    public bool isMask;
    private bool disableHover;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
        
        // calculate scaling factor based on current screen size and reference screen size
        float scalingFactor = Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);

        // adjust targetLocalDisplacement based on scaling factor
        targetLocalDisplacement *= scalingFactor;
        
        //this scaling factor stuff is necessary because the bigger the screen is, the more units our UI has to cross
        //to get to the intended value. because the game is intended at an aspect ratio of 16:9, it doesnt matter what our 
        //reference resolution is, as long as it's 16:9 (all intended values are based off of 1920x1080 so pls dont change)
    }

    private void Start()
    {
        homePosition = transform.position;
        endingSpringPosition = homePosition + targetLocalDisplacement;
        currentSpringPosition = homePosition;
        targetPosition = homePosition;
    }


    private void OnEnable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene += BounceInAndOut;
    }

    private void OnDisable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene -= BounceInAndOut;
    }
    
    private async void BounceInAndOut()
    {
        targetPosition = endingSpringPosition;
        await Task.Delay(TimeSpan.FromSeconds(7));
        targetPosition = homePosition;
    }
    
    public void MoveToEndPosition()
    {
        if (disableHover) return;
        targetPosition = endingSpringPosition;
    }
    
    public void MoveToStartPosition()
    {
        if (disableHover) return;
        targetPosition = homePosition;
    }

    public void SetSpringPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
    
    public void DisableHover()
    {
        disableHover = true;
    }

    private void Update()
    {
        endingSpringPosition = homePosition + targetLocalDisplacement;
        transform.position = new Vector3(currentSpringPosition.x, currentSpringPosition.y, currentSpringPosition.z);
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.unscaledDeltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentSpringPosition.x, ref vel.x, targetPosition.x, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentSpringPosition.y, ref vel.y, targetPosition.y, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentSpringPosition.z, ref vel.z, targetPosition.z, springParams);
    }
}
