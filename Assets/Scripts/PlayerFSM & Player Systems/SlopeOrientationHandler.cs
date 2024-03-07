using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines.Primitives;
using UnityEngine;

public class SlopeOrientationHandler : MonoBehaviour
{
    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private Transform playerModelTransform;
    [SerializeField] private Transform inputTurningTransform;

    #region Orientation Values

    [SerializeField] private float baseSlopeOrientationSpeed;
    float slopeOrientationSpeed;
    public float slopeDownDetectionDistance;
    public float slopeForwardDetectionDistance;
    [Tooltip("The distance from the center of the player to the left and right raycast origins. These are used to detect the slope.")]
    public float slopeRayOffsetFromZ;

    public float slopeRayOffsetFromX;
    

    [Tooltip("Used when your detection raycasts indicate that you are about to hit the ground at full force on your face/side/back. This refers to the speed at which the player will re-orient to match the ground")]
    public float emergencySlopeReOrientSpeed;
    public float airReOrientSpeed;

    public Quaternion targetRotation;
    
    #endregion


    private void Start()
    {
        slopeOrientationSpeed = baseSlopeOrientationSpeed;
    }

    public void SetOrientationSpeed(float speed)
    {
        slopeOrientationSpeed = speed;
    }

    public void ResetOrientationSpeed()
    {
        slopeOrientationSpeed = baseSlopeOrientationSpeed;
    }

    public void OrientToSlope()
    {
        Vector3 averageNormal = (playerBase.forwardLeftSlopeHit.normal + playerBase.forwardRightSlopeHit.normal + 
                                 playerBase.backLeftSlopeHit.normal +
                                 playerBase.backRightSlopeHit.normal).normalized;

        // stores perpendicular angle into targetRotation
        targetRotation = Quaternion.FromToRotation(playerBase.transform.up, averageNormal) 
                                    * playerBase.transform.rotation;

        // Lerp to the desired rotation
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation,
            Time.fixedDeltaTime * slopeOrientationSpeed);
    }
    
    public void ChangePivot(Transform parentTransform, Vector3 newPivot)
    {
        Dictionary<Transform, Vector3> cachedChildPositions = new Dictionary<Transform, Vector3>();
        foreach (Transform child in parentTransform)
        {
            cachedChildPositions[child] = child.position;
        }
        
        parentTransform.position = newPivot;

        foreach (KeyValuePair<Transform, Vector3> childPosition in cachedChildPositions)
        {
            childPosition.Key.position = childPosition.Value;
        }
    }
    
    public void OrientFromExtensions() // should refactor into a coroutine to do this, so we are locked into orienting
    {
        Quaternion targetRotation = Quaternion.FromToRotation(playerBase.transform.up, Vector3.up) 
                                    * playerBase.transform.rotation;

        // Lerp to the desired rotation
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation,
            Time.fixedDeltaTime * emergencySlopeReOrientSpeed);
    }
    /// <summary>
    /// Slowly re-orients the player mid-air to be upright. Meant to be used in FixedUpdate.
    /// </summary>
    public void ReOrient()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(playerBase.transform.up, Vector3.up) * playerBase.transform.rotation;
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation, Time.fixedDeltaTime * airReOrientSpeed);
    }
}
