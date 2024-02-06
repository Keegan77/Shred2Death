using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeOrientationHandler : MonoBehaviour
{
    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private Transform playerModelTransform;
    [SerializeField] private Transform inputTurningTransform;

    #region Orientation Values
    public float slopeOrientationSpeed;
    public float slopeDownDetectionDistance;
    public float slopeForwardDetectionDistance;
    [Tooltip("The distance from the center of the player to the left and right raycast origins. These are used to detect the slope.")]
    public float slopeRayOffsetFromZ;

    [Tooltip("The height offset of the ground extension raycasts. These raycasts are used to detect if the player has fallen over, and will re-orient the player if the player has fallen over and they are about to hit the ground")]
    public float extensionRayHeightOffset;

    public float slopeRayOffsetFromX;
    

    [Tooltip("Used when your detection raycasts indicate that you are about to hit the ground at full force on your face/side/back. This refers to the speed at which the player will re-orient to match the ground")]
    public float emergencySlopeReOrientSpeed;
    public float airReOrientSpeed;
    
    #endregion
    
    public void OrientToSlope()
    {
        Vector3 averageNormal = (playerBase.forwardLeftSlopeHit.normal + playerBase.forwardRightSlopeHit.normal + 
                                 playerBase.backLeftSlopeHit.normal +
                                 playerBase.backRightSlopeHit.normal).normalized;

        // stores perpendicular angle into targetRotation
        Quaternion targetRotation = Quaternion.FromToRotation(playerBase.transform.up, averageNormal) 
                                    * playerBase.transform.rotation;

        // Lerp to the desired rotation
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation,
            Time.fixedDeltaTime * slopeOrientationSpeed);
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
