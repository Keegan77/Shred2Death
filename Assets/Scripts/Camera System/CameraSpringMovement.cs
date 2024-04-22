using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringMovement : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParamsHoz;
    SpringUtils.tDampedSpringMotionParams springParamsVert;

    [SerializeField] private float frequencyHoz, frequencyVert;
    [SerializeField] private float dampingRatioHoz, dampingRatioVert;
    
    public Transform targetTransform;
    private float currentXPosition, currentYPosition, currentZPosition;
    private float velX, velY, velZ;

    private void Awake()
    {
        springParamsHoz = new SpringUtils.tDampedSpringMotionParams();
        springParamsVert = new SpringUtils.tDampedSpringMotionParams();
    }

    private void Start()
    {
        currentXPosition = transform.position.x;
    }
    
    private void Update()
    {
        transform.position = new Vector3(targetTransform.position.x, currentYPosition, targetTransform.position.z);
        transform.rotation = targetTransform.rotation;
        SpringUtils.CalcDampedSpringMotionParams(ref springParamsHoz, Time.unscaledDeltaTime, frequencyHoz, dampingRatioHoz);
        SpringUtils.CalcDampedSpringMotionParams(ref springParamsVert, Time.unscaledDeltaTime, frequencyVert, dampingRatioVert);
        
        SpringUtils.UpdateDampedSpringMotion(ref currentXPosition, ref velX, targetTransform.position.x, springParamsHoz);
        SpringUtils.UpdateDampedSpringMotion(ref currentYPosition, ref velY, targetTransform.position.y, springParamsVert);
        SpringUtils.UpdateDampedSpringMotion(ref currentZPosition, ref velZ, targetTransform.position.z, springParamsHoz);
    }
}
