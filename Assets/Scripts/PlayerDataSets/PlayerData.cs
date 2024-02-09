using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataSet")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Values")]
    public float baseMovementSpeed;

    public float baseBoostSpeed;
    public float boostDuration;
    public float baseJumpForce;
    public float baseTurnSharpness;
    public float deAccelerationSpeed;
    
    
    
    [Header("Airborne Values (Non-Half Pipe)")]
    public float airForwardForce;

    [Header("Nosedive Values")] 
    public float noseDiveFallForce;

    public float maxNoseDiveVelocity;
    
    
    [Header("Grinding Values")]
    public float minGrindSpeed;
    public float maxGrindSpeed;
    public float grindSpeedAdditive;
    public float grindPositioningOffset;
    public float grindTurnSharpness;
    public float railSnapTime;
    public float railSnapDistance;

    
    
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
    
    
    
    [Header("Drift Phase Timings")] 
    [Tooltip("Amount of time in seconds before the drift will grant you a speed boost by sending you to the next drift phase")]
    public float lightDriftTime;
    [Tooltip("Amount of time in seconds before each drift boost phase will send you to the next drift phase")]
    public float driftPhaseTime;
    
    
    
    [Header("Slope Orientation Settings")]

    [Tooltip("X is min, Y is max. If the slope is within this range, the player will not be able to exert a forward force. Used for preventing the player from using forward force up slopes that are too steep")]
    public Vector2 slopeRangeWherePlayerCantMove;

    public float slopedUpSpeedMult;

    public float slopedDownSpeedMult;

    [Header("Camera settings")] public float defaultFOV;
    public float maxFOV;

}
