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
    private float vel;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
    }

    private void Start()
    {
        currentUniformScale = transform.localScale.magnitude;
    }
    
    public void SetSpringyScale(float scale)
    {
        targetUniformScale = scale;
    }
    
    private void Update()
    {
        transform.localScale = new Vector3(currentUniformScale, currentUniformScale, currentUniformScale);
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.deltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentUniformScale, ref vel, targetUniformScale, springParams);
    }
}
