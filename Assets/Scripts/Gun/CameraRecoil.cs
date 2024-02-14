using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    //rotations
    private Vector3 currentRotation, targetRotation;
    [SerializeField] GunfireHandler gunScript;
    //recoil values
    
    private void Update()
    {
        targetRotation  =  Vector3.Lerp(targetRotation, Vector3.zero, 
            gunScript.GetCurrentGunData().returnSpeed * Time.deltaTime);
        
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, 
            gunScript.GetCurrentGunData().snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void FireRecoil()
    {
        targetRotation += new Vector3(-gunScript.GetCurrentGunData().recoilX,
            Random.Range(gunScript.GetCurrentGunData().recoilY, -gunScript.GetCurrentGunData().recoilY),
            Random.Range(gunScript.GetCurrentGunData().recoilZ, -gunScript.GetCurrentGunData().recoilZ));
    }
}
