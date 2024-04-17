using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindButtonBehaviour : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;

    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;

    public float minUniformScale;
    private float targetUniformScale;
    public float maxUniformScale;
    private float currentUniformScale;
    private float vel, vel1, vel2, vel3;
    private float targetX, targetY, targetZ;
    private float currentX, currentY, currentZ;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
    }

    private void Start()
    {
        currentUniformScale = transform.localScale.magnitude;
        SetCurrentPosition(transform.position);
    }
    
    public void SetSpringyScale(float scale)
    {
        targetUniformScale = scale;
    }

    public void SetSpringyPosition(Vector3 position)
    {
        targetX = position.x;
        targetY = position.y;
        targetZ = position.z;
    }
    
    public void SetCurrentPosition(Vector3 position)
    {
        currentX = position.x;
        currentY = position.y;
        currentZ = position.z;
    }
    
    private void Update()
    {
        transform.localScale = new Vector3(currentUniformScale, currentUniformScale, currentUniformScale);
        transform.position = new Vector3(currentX, currentY, currentZ);
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.deltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentUniformScale, ref vel, targetUniformScale, springParams);
        
        SpringUtils.UpdateDampedSpringMotion(ref currentX, ref vel1, targetX, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentY, ref vel2, targetY, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentZ, ref vel3, targetZ, springParams);
    }
}
