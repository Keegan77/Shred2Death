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
    
    /*public void OrientToSlope()
    {
        float currentYRotation = playerBase.transform.rotation.eulerAngles.y;
        // Cast raycasts from the position of each wheel
        // Calculate wheelbase and trackWidth
        Vector3 frontCenter = (playerBase.forwardLeftRayOrigin + playerBase.forwardRightRayOrigin) / 2;
        Vector3 backCenter = (playerBase.backLeftRayOrigin + playerBase.backRightRayOrigin) / 2;
        Vector3 leftCenter = (playerBase.forwardLeftRayOrigin + playerBase.backLeftRayOrigin) / 2;
        Vector3 rightCenter = (playerBase.forwardRightRayOrigin + playerBase.backRightRayOrigin) / 2;

        float wheelbase = Vector3.Distance(frontCenter, backCenter);
        float trackWidth = Vector3.Distance(leftCenter, rightCenter);
        // Get the Y position of each raycast hit
        float frontLeftY = playerBase.forwardLeftSlopeHit.point.y;
        float frontRightY = playerBase.forwardRightSlopeHit.point.y;
        float backLeftY = playerBase.backLeftSlopeHit.point.y;
        float backRightY = playerBase.backRightSlopeHit.point.y;

        // Calculate the difference in Y positions between the front and back wheels, and between the left and right wheels
        float frontBackDifference = ((backLeftY + backRightY) / 2) - ((frontLeftY + frontRightY) / 2);
        float leftRightDifference = ((frontRightY + backRightY) / 2) - ((frontLeftY + backLeftY) / 2);

        // Calculate the rotation angles around the X and Z axes
        float xRotation = Mathf.Atan2(frontBackDifference, wheelbase) * Mathf.Rad2Deg;
        float zRotation = Mathf.Atan2(leftRightDifference, trackWidth) * Mathf.Rad2Deg;
        // Calculate the target rotation
        
        Quaternion targetRotation = Quaternion.identity;
        //targetRotation *= Quaternion.AngleAxis(xRotation, transform.right);
        //targetRotation *= Quaternion.AngleAxis(zRotation, transform.forward);
        targetRotation = Quaternion.Euler(xRotation, currentYRotation, zRotation);
        
        //use fromtorotation to get the actual target rotation based on transform.up
        //targetRotation = Quaternion.FromToRotation(playerBase.transform.up, targetRotation * Vector3.up) * playerBase.transform.rotation;
        

        // Apply the target rotation
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation, Time.fixedDeltaTime * slopeOrientationSpeed);
        // Apply these rotation angles to the skateboard
        //playerBase.transform.rotation = Quaternion.Euler(playerBase.transform.rotation.eulerAngles.x, currentYRotation, playerBase.transform.rotation.eulerAngles.z);
        
        Debug.Log(xRotation + " " + zRotation);
        //playerBase.transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, zRotation);
    }*/
    
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
