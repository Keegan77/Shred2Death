using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataSet")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Values")]
    public float baseMovementSpeed;
    public float baseJumpForce;
    public float baseTurnSharpness;
    public float deAccelerationSpeed;

    [Header("Airborne Values (Non-Half Pipe)")]
    public float airForwardForce;
    [Header("Grinding Values")]
    public float baseGrindingSpeed;
    public float grindPositioningOffset;
    public float grindTurnSharpness;

    [Header("Drifting Values")] 
    public float baseDriftForce;
    public float baseDriftTurnSharpness;
    [Tooltip("Amount of rotation applied to the player model during a drift.")]
    public float driftRotationalOffset;
    public float baseDriftBoost;
    [Tooltip("Amount of boost added per consecutive drift phase")]
    public float driftBoostAdditive;
    [Tooltip("Pressing Left or Right while in a drift will increase or decrease the sharpness of the drift by this amount")]
    public float inputExtraDriftTurnSharpness;
    [Tooltip("Used as the T parameter while lerping the drift transform to 90 or 270")]
    public float playerModelRotationSpeed;

    public float railSnapTime;

    [Header("Drift Phase Timings")] 
    [Tooltip("Amount of time in seconds before the drift will grant you a speed boost by sending you to the next drift phase")]
    public float lightDriftTime;
    [Tooltip("Amount of time in seconds before each drift boost phase will send you to the next drift phase")]
    public float driftPhaseTime;
    
    [Header("Slope Orientation Settings")]
    public float slopeOrientationSpeed;
    public float slopeDownDetectionDistance;
    public float slopeForwardDetectionDistance;
    [Tooltip("The distance from the center of the player to the left and right raycast origins. These are used to detect the slope.")]
    public float slopeRayOffsetFromMid;
    [Tooltip("X is min, Y is max. If the slope is within this range, the player will not be able to exert a forward force. Used for preventing the player from using forward force up slopes that are too steep")]
    public Vector2 slopeRangeWherePlayerCantMove;
    [Tooltip("Used when your detection raycasts indicate that you are about to hit the ground at full force on your face/side/back. This refers to the speed at which the player will re-orient to match the ground")]
    public float emergencySlopeReOrientSpeed;

    public float airReOrientSpeed;

    public float slopedUpSpeedMult;

    public float slopedDownSpeedMult;

}
