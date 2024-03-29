using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataSet")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Values")]
    public float baseMovementSpeed;

    [Tooltip("The style level at which the grounded boost unlocks")]
    public int groundedBoostStyleLevel;
    public float baseBoostSpeed;
    public float boostDuration;
    public float baseJumpForce;
    //public float baseTurnSharpness; 150
    public float deAccelerationSpeed;
    
    public Vector2 minMaxTurnSharpness;
    public float speedMagnitudeThresholdForMaxTurnSharpness;
    public AnimationCurve turnSharpnessCurve;
    
    [Header("Acceleration")]
    [Tooltip("Minimum speed used as the starting acceleration force. Over time increases until it's at base movement speed")]
    public float minSpeed;
    [Tooltip("Time it takes to reach base movement speed")]
    public float accelTime;
    [Tooltip("The curve used to determine the acceleration over time")]
    public AnimationCurve accelerationCurve;
    
    
    [Header("Airborne Values")]
    public float airForwardForce;
    public float airTiltRange;
    public float airBoostImpulseForce;
    public float airBoostCooldownSeconds;
    [Tooltip("The style level at which the air boost unlocks")]
    public int airBoostStyleLevel;

    public float halfPipeAirTurnAmount;


    [Header("Nosedive Values")] 
    public float noseDiveFallForce;
    [Tooltip("Max fall speed")]
    public float maxNoseDiveVelocity;
    public float noseDiveRotationSpeed;
    
    
    [Header("Grinding Values")]
    [Tooltip("Grind speed is dependent on the speed of the player when they hit the grind rail. If that speed is under the min speed, the min speed will be used.")]
    public float minGrindSpeed;
    [Tooltip("Grind speed is dependent on the speed of the player when they hit the grind rail. If that speed is over the max speed, the max speed will be used.")]
    public float maxGrindSpeed;
    [Tooltip("The amount of force added to the player per drift phase.")]
    public float grindSpeedAdditive;

    [Tooltip("The turn amount of grinding, should be probably slightly higher than the base turn sharpness or something. Play around with it!")]
    public float grindTurnSharpness;
    [Tooltip("Amount of time it takes to snap to the grind rail")]
    public float railSnapTime;
    [Tooltip("Radius from the player where the player will detect and snap to the nearest grind rail.")]
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
    [Tooltip("Used as the T parameter while lerping the drift transform to 90 or 270. Don't worry about this too much")]
    public float playerModelRotationSpeed;
    
    
    
    [Header("Drift Phase Timings")] 
    [Tooltip("Amount of time in seconds before the drift will grant you a speed boost. " +
             "Letting go of drift before this value in seconds results in no speed boost.")]
    public float lightDriftTime;
    [Tooltip("Amount of time in seconds before each drift boost phase will send you to the next drift phase")]
    public float driftPhaseTime;
    
    [Header("Slope Orientation Settings")]

    [Tooltip("X is min, Y is max. If the slope is within this range, the player will not be able to exert a forward " +
             "force. Used for preventing the player from using forward force up slopes that are too steep")]
    public Vector2 slopeRangeWherePlayerCantMove;

    [Tooltip("The higher this value is set, the less speed the player will have up slopes.")]
    public float slopedUpSpeedMult;
    [Tooltip("The lower this value is set, the more speed the player will have down slopes.")]
    public float slopedDownSpeedMult;

    [Header("Camera settings")] public float minFOV;
    public float maxFOV;
    [Tooltip("Aim down sights speed")]
    public float adsSpeed;
    
    [Header("State Exit Style Points")]
    public float driftStylePoints;

    
    [Header("Burn Bounce Values")] 
    public float burnForce;
    public float extraBurnVerticalForce;
    public float burnBounceCooldown;

}
