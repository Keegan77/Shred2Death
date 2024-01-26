using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataSet")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Values")]
    public float baseMovementSpeed;
    public float baseJumpForce;
    public float baseTurnSharpness;
    public float deAccelerationSpeed;

    [Header("Grinding Values")]
    public float baseGrindingSpeed;
    public float grindPositioningOffset;

    [Header("Drifting Values")] 
    public float baseDriftForce;
    public float baseDriftTurnSharpness;
    
    [Header("Slope Orientation Settings")]
    public float slopeOrientationSpeed;

    public float slopeDetectionDistance;
    [Tooltip("The distance from the center of the player to the left and right raycast origins. These are used to detect the slope.")]
    public float slopeRayOffsetFromMid;
    [Tooltip("X is min, Y is max. If the slope is within this range, the player will not be able to exert a forward force. Used for preventing the player from using forward force up slopes that are too steep")]
    public Vector2 slopeRangeWherePlayerCantMove;

    public float slopedUpSpeedMult;

    public float slopedDownSpeedMult;

}
