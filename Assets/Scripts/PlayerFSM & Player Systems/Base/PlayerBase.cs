using System;
using Dreamteck.Splines;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Serialization;


public class PlayerBase : MonoBehaviour
{
    [Header("Component References")]
    public Rigidbody rb;
    [SerializeField] private Collider skateboardCollider;
    public Transform inputTurningTransform, playerModelTransform;
    [SerializeField] private Transform raycastPoint;
    private SplineComputer currentSpline;
    private double splineCompletionPercent;
    
    //Player Data
    [Header("Player Data")] [Tooltip("Holds all of the player's base movement values.")]
    public PlayerData playerData;
    //Variables which hold calculated values based on their base constants.
    private float movementSpeed;
    private float turnSharpness;
    
    //caching the move input that was held when drift started
    
    
    //state machine
    public PlayerStateMachine stateMachine { get; private set; }
    //concrete states
    public PlayerSkatingState skatingState;
    public PlayerAirborneState airborneState;
    public PlayerHalfpipeState halfPipeState;
    public PlayerGrindState grindState;
    public PlayerDriftState driftState;
    
    
    float jumpInput;

#region Unity Abstracted Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StateMachineSetup();
    }
    
    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
    }
    
    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        stateMachine.currentState.StateTriggerEnter(other);
    }
    
    private void OnTriggerStay(Collider other)
    {
        stateMachine.currentState.StateTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        stateMachine.currentState.StateTriggerExit(other);
    }

    private void OnCollisionStay(Collision other)
    {
        stateMachine.currentState.StateCollisionEnter(other);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 backRayOrigin = raycastPoint.position - playerModelTransform.forward * playerData.slopeRayOffsetFromMid;
        Vector3 forwardRayOrigin = raycastPoint.position + playerModelTransform.forward * playerData.slopeRayOffsetFromMid;
       
        Vector3 leftRayOrigin = raycastPoint.position - playerModelTransform.right * playerData.slopeRayOffsetFromMid;
        Vector3 rightRayOrigin = raycastPoint.position + playerModelTransform.right * playerData.slopeRayOffsetFromMid;
        
        Gizmos.DrawLine(backRayOrigin, backDownwardSlopeHit.point/*backRayOrigin + Vector3.down * playerData.slopeDownDetectionDistance*/);
        Gizmos.DrawLine(forwardRayOrigin, forwardDownwardSlopeHit.point/*forwardRayOrigin + Vector3.down * playerData.slopeDownDetectionDistance*/);
        
        Gizmos.DrawLine(backRayOrigin, backwardSlopeHit.point);
        Gizmos.DrawLine(forwardRayOrigin, forwardSlopeHit.point/*forwardRayOrigin - Vector3.forward * playerData.slopeForwardDetectionDistance*/);
        
        Gizmos.DrawLine(leftRayOrigin, leftSlopeHit.point /*leftRayOrigin + Vector3.down * playerData.slopeDownDetectionDistance*/);
        Gizmos.DrawLine(rightRayOrigin, rightSlopeHit.point /*rightRayOrigin + Vector3.down * playerData.slopeDownDetectionDistance*/);
    }
    
#endregion

#region Movement Methods

    /// <summary>
    /// Will exert a force forward if the player's slope isn't too steep. Meant to be used in FixedUpdate.
    /// </summary>
    public void SkateForward()
    {
        CalculateSpeedVector();
        
        Vector2 maxSlopeRange = new Vector2(playerData.slopeRangeWherePlayerCantMove.x + 90, playerData.slopeRangeWherePlayerCantMove.y + 90);
        
        // calculates the angle between the player's forward direction and the world's down direction
        float angleWithDownward = GetOrientationWithDownward();

        //Debug.Log(angleWithDownward);
        
        bool isFacingUpward = angleWithDownward > maxSlopeRange.x && angleWithDownward < maxSlopeRange.y;
        
        if (isFacingUpward) return;
        
        rb.AddForce(inputTurningTransform.forward * (movementSpeed * (InputRouting.Instance.GetMoveInput().y > 0 ? InputRouting.Instance.GetMoveInput().y : 0)), ForceMode.Acceleration); // Only adds force if
        // the player is not
        // on a slope that is
        // too steep.
    }

    public void OllieJump()
    {
        JumpOffRail();
        if (CheckGround())
        {
            rb.AddRelativeForce(transform.up * playerData.baseJumpForce, ForceMode.Impulse);
        }
    }
    
    private void JumpOffRail() // whole method is placeholder for testing
    {
        if (stateMachine.currentState != grindState) return;
        var speed = GetComponent<SplineFollower>().followSpeed;
        SetRBKinematic(false);
        GameObject.Destroy(GetComponent<SplineFollower>());
        rb.AddForce(transform.forward * speed * 100); 
        rb.AddForce(Vector3.up * 20);
        stateMachine.SwitchState(airborneState);
    }
    
    /// <summary>
    /// Handles turning the player model with left and right input. Rotating the player works best for the movement we
    /// are trying to achieve, as movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    public void TurnPlayer() // Rotates the PLAYER MODEL TRANSFORM. We must work with 2 transforms to achieve the desired effect.
    {
        inputTurningTransform.Rotate(0, turnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime, 0, Space.Self);
    }
    
    private void CalculateSpeedVector()
    {
        float offset = rb.velocity.y;
        
        if (rb.velocity.y > 0)
        {
            offset = -rb.velocity.y * playerData.slopedUpSpeedMult;
        }
        else if (rb.velocity.y < 0)
        {
            offset = rb.velocity.y / playerData.slopedDownSpeedMult;
        }
        // Get the rotation around the x-axis, ranging from -90 to 90
        
        movementSpeed = playerData.baseMovementSpeed + offset;
        //Debug.Log(movementSpeed);
    }

    public void CalculateTurnSharpness()
    {
        if (rb.velocity.magnitude < 20) turnSharpness = playerData.baseTurnSharpness;
        else turnSharpness = playerData.baseTurnSharpness / (rb.velocity.magnitude / 15);
    }

    private RaycastHit backDownwardSlopeHit, forwardDownwardSlopeHit, leftSlopeHit, rightSlopeHit, forwardSlopeHit, backwardSlopeHit;
    public void OrientToSlope()
    {
        if (CheckGround())
        {
            Vector3 averageNormal = (backDownwardSlopeHit.normal + forwardDownwardSlopeHit.normal + leftSlopeHit.normal +
                             rightSlopeHit.normal).normalized;

            // stores perpendicular angle into targetRotation
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;

            // Lerp to the desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                Time.fixedDeltaTime * playerData.slopeOrientationSpeed);
        }
    }

    public void OrientFromExtensions()
    {
        if (CheckGroundExtensions())
        {
            Debug.Log($"Forward slope hit normal: {forwardSlopeHit.normal}");
            
            Vector3 averageNormal = (forwardSlopeHit.normal).normalized;
            
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;

            // Lerp to the desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * playerData.slopeOrientationSpeed);
        }
    }

    /// <summary>
    /// Slowly re-orients the player mid-air to be upright. Meant to be used in FixedUpdate.
    /// </summary>
    public void ReOrient()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * playerData.airReOrientSpeed);
    }
    
    /// <summary>
    /// De-accelerates the player by a fixed value. As long as the de-acceleration value is less than the acceleration
    /// value, the desired effect will work properly. Meant to be used in FixedUpdate.
    /// </summary>
    public void DeAccelerate() // Add Force feels too floaty, used on every frame to counteract the force.
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), playerData.deAccelerationSpeed);
    }


#endregion


#region Helper Methods, Getters, & Setters
    public void SetCurrentSpline(SplineComputer spline, SplineSample splineHitPoint)
    {
        currentSpline = spline;
        splineCompletionPercent = splineHitPoint.percent;
    }

    public SplineComputer GetCurrentSpline()
    {
        return currentSpline;
    }
        
    public double GetSplineCompletionPercent()
    {
        return splineCompletionPercent;
        //splineCompletionPercent = currentSpline.Project(transform.position).percent;
    }

    public void SetRBKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }
    
    /// <summary>
    /// [DEPRECATED: USE LOCALEULERANGLES INSTEAD]
    /// Translates eulerAngles from 0 - +360, to -180 - +180. Makes eulerAngles easier to work with, logically.
    /// Rotations should never be applied with this method, as it will cause weirdness. This is simply for getting
    /// eulerAngle values in a range that makes sense. 
    /// </summary>
    private float TranslateEulersToRange180(float eulerAngle)
    {
        return eulerAngle > 180 ? eulerAngle - 360 : eulerAngle;
    }
    
    public bool CheckGround()
    {
        var layerMask = (1 << LayerMask.NameToLayer("Ground"));
        
        Vector3 backRayOrigin = raycastPoint.position - playerModelTransform.forward * playerData.slopeRayOffsetFromMid;
        Vector3 forwardRayOrigin = raycastPoint.position + playerModelTransform.forward * playerData.slopeRayOffsetFromMid;
        
        Vector3 leftRayOrigin = raycastPoint.position - playerModelTransform.right * playerData.slopeRayOffsetFromMid;
        Vector3 rightRayOrigin = raycastPoint.position + playerModelTransform.right * playerData.slopeRayOffsetFromMid;
        
        bool backDownRayHit = Physics.Raycast(backRayOrigin, -playerModelTransform.up, out backDownwardSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        bool forwardDownRayHit = Physics.Raycast(forwardRayOrigin, -playerModelTransform.up, out forwardDownwardSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        

        
        bool leftRayHit = Physics.Raycast(leftRayOrigin, -playerModelTransform.up, out leftSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        bool rightRayHit = Physics.Raycast(rightRayOrigin, -playerModelTransform.up, out rightSlopeHit, playerData.slopeDownDetectionDistance, layerMask);

        return backDownRayHit || forwardDownRayHit || leftRayHit || rightRayHit;
    }

    /// <summary>
    /// This method is for checking a raycast forward from the skateboard, will be used for checking if the player
    /// has fallen flat on their face.
    /// </summary>
    /// <returns>True if Raycast hits ground</returns>
    public bool CheckGroundExtensions()
    {
        var layerMask = (1 << LayerMask.NameToLayer("Ground"));
        
        Vector3 backRayOrigin = raycastPoint.position - inputTurningTransform.forward * playerData.slopeRayOffsetFromMid;
        Vector3 forwardRayOrigin = raycastPoint.position + inputTurningTransform.forward * playerData.slopeRayOffsetFromMid;
        
        bool backRayHit = Physics.Raycast(backRayOrigin, -inputTurningTransform.forward, out backwardSlopeHit, playerData.slopeForwardDetectionDistance, layerMask);
        bool forwardRayHit = Physics.Raycast(forwardRayOrigin, inputTurningTransform.forward, out forwardSlopeHit, playerData.slopeForwardDetectionDistance, layerMask);
        
        return forwardRayHit;
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }
    
    public float GetOrientationWithDownward()
    {
        return Vector3.Angle(inputTurningTransform.forward, Vector3.down);
    }
    #endregion
    
    private void StateMachineSetup()
    {
        stateMachine = new PlayerStateMachine();
        skatingState = new PlayerSkatingState(this, stateMachine);
        airborneState = new PlayerAirborneState(this, stateMachine);
        halfPipeState = new PlayerHalfpipeState(this, stateMachine);
        grindState = new PlayerGrindState(this, stateMachine);
        driftState = new PlayerDriftState(this, stateMachine);
        stateMachine.Init(skatingState);
    }
    
    private void OnEnable()
    {
        InputRouting.Instance.input.Player.Jump.performed += ctx => OllieJump();
    }

    private void OnDisable()
    {
        InputRouting.Instance.input.Player.Jump.performed -= ctx => OllieJump();
    }
    
}
