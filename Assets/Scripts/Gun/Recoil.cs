using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recoil : MonoBehaviour
{
    //rotations
    private Vector3 currentRotation, targetRotation;
    private Vector3 oldStartRotation;
    private Vector3 startRotation;
    [SerializeField] GunfireHandler gunScript;
    
    [Header("Recoil return settings")]
    
    [Tooltip("Speed of the recoil snap to the set X Y and Z values.")]
    [SerializeField] private float snappiness;
    
    [Tooltip("Speed of the recoil return to 0, 0, 0.")]
    [SerializeField] private float returnSpeed;
    //recoil values
    private void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
    }

    private void OnEnable()
    {
        ActionEvents.IntermediaryAbilityStateEnter += ResetStartRotation;
    }

    private void Update()
    {
        targetRotation  =  Vector3.Lerp(targetRotation, startRotation, 
            returnSpeed * Time.deltaTime);
        
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, 
            snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    
    public void ChangeStartLocation(Vector3 newStartLocation)
    {
        oldStartRotation = startRotation;
        startRotation = newStartLocation;
    }
    
    public void ResetStartRotation()
    {
        startRotation = Vector3.zero;
    }

    public void FireRecoil(float recoilX, float recoilY, float recoilZ)
    {
        targetRotation += new Vector3(-recoilX,
                       Random.Range(recoilY, -recoilY),
                       Random.Range(recoilZ, -recoilZ));
    }
}
