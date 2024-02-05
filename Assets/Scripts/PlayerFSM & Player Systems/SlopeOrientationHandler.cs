using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeOrientationHandler
{
    private PlayerBase playerBase;
    private PlayerData playerData;
    private Transform playerModelTransform;
    private Transform inputTurningTransform;
    public SlopeOrientationHandler(PlayerBase playerBase, PlayerData playerData, Transform playerModelTransform,
        Transform inputTurningTransform)
    {
        this.playerBase = playerBase;
        this.playerData = playerData;
        this.playerModelTransform = playerModelTransform;
        this.inputTurningTransform = inputTurningTransform;
    }
    
    // Update is called once per frame
    
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
            Time.fixedDeltaTime * playerData.slopeOrientationSpeed);
    }
    
    public void OrientFromExtensions() // should refactor into a coroutine to do this, so we are locked into orienting
    {
        Debug.Log($"Forward slope hit normal: {playerBase.forwardSlopeHit.normal}");
        
        Vector3 averageNormal = (playerBase.forwardSlopeHit.normal).normalized;
        
        Quaternion targetRotation = Quaternion.FromToRotation(playerBase.transform.up, Vector3.up) 
                                    * playerBase.transform.rotation;

        // Lerp to the desired rotation
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation,
            Time.fixedDeltaTime * playerData.emergencySlopeReOrientSpeed);
    }
    /// <summary>
    /// Slowly re-orients the player mid-air to be upright. Meant to be used in FixedUpdate.
    /// </summary>
    public void ReOrient()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(playerBase.transform.up, Vector3.up) * playerBase.transform.rotation;
        playerBase.transform.rotation = Quaternion.Slerp(playerBase.transform.rotation, targetRotation, Time.fixedDeltaTime * playerData.airReOrientSpeed);
    }
}
