using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositionMover : MonoBehaviour
{
    [SerializeField] private Transform[] transformsToMove;
    [SerializeField] private Transform[] cPoseTransformReferences;
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private void Start()
    {
        foreach (var transform in transformsToMove)
        {
            originalPositions.Add(transform, transform.localPosition);
            originalRotations.Add(transform, transform.localRotation);
        }
    }

    public void SwitchToChristPosition()
    {
        foreach (var originTransform in transformsToMove)
        {
            foreach (var cPoseRef in cPoseTransformReferences)
            {
                if (cPoseRef.CompareTag(originTransform.tag))
                {
                    originTransform.localPosition = cPoseRef.localPosition;
                    originTransform.localRotation = cPoseRef.localRotation;
                    if (originTransform.CompareTag("LeftHandTarget") || originTransform.CompareTag("RightHandTarget"))
                    {
                        originTransform.GetComponent<Recoil>().ChangeStartLocation(cPoseRef.localEulerAngles);
                    }
                }
            }
        }
    }
    
    public void ResetTransformPositions()
    {
        Debug.Log("Resetting Transform Positions");
        foreach (var transform in transformsToMove)
        {
            Debug.Log("Resetting " + transform.name);
            transform.localPosition = originalPositions[transform];
            transform.localRotation = originalRotations[transform];
        }
    }
}
