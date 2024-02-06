using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCameraWithInput : MonoBehaviour
{
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float zRotationAmount;
    private float rotationAmt;
    
    private void Update()
    {
        rotationAmt = zRotationAmount * -InputRouting.Instance.GetMoveInput().x;
        targetRotation = Quaternion.Euler(0, 0, rotationAmt);
    }

    void LateUpdate()
    {
        // Slerp from the current rotation to the target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
