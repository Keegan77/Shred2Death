using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToForward : MonoBehaviour
{
    [SerializeField] private Transform mainRotationTransform;
    [SerializeField] private Transform additionalRotationTransform;
    private Quaternion targetRotation;
    [SerializeField] float slerpSpeed = 0.1f;

    // Update is called once per frame
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Calculate the target rotation
        float combinedYRotation = mainRotationTransform.localEulerAngles.y + additionalRotationTransform.localEulerAngles.y;
        
        targetRotation = Quaternion.Euler(0, combinedYRotation, 0);

        // Slerp from the current rotation to the target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, slerpSpeed * Time.deltaTime);
    }
}
