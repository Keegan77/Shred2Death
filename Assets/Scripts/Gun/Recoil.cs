using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    //rotations
    private Vector3 currentRotation, targetRotation;
    [SerializeField] GunfireHandler gunScript;
    
    [Header("Recoil return settings")]
    
    [Tooltip("Speed of the recoil snap to the set X Y and Z values.")]
    [SerializeField] private float snappiness;
    
    [Tooltip("Speed of the recoil return to 0, 0, 0.")]
    [SerializeField] private float returnSpeed;
    //recoil values
    
    private void Update()
    {
        targetRotation  =  Vector3.Lerp(targetRotation, Vector3.zero, 
            returnSpeed * Time.deltaTime);
        
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, 
            snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void FireRecoil(float recoilX, float recoilY, float recoilZ)
    {
        targetRotation += new Vector3(-recoilX,
                       Random.Range(recoilY, -recoilY),
                       Random.Range(recoilZ, -recoilZ));
    }
}
