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
    [SerializeField] private Transform extensionRaycastPoint;
    private SplineComputer currentSpline;
    private double splineCompletionPercent;
    
    //Player Data
    [Header("Player Data")] [Tooltip("Holds all of the player's base movement values.")]
    public PlayerData playerData;
    //Variables which hold calculated values based on their base constants.
    private float movementSpeed;
    private float turnSharpness;
    
    //raycast slope detection origin points
    [HideInInspector] public Vector3 forwardLeftRayOrigin, forwardRightRayOrigin, backLeftRayOrigin, backRightRayOrigin;
    [HideInInspector] public Vector3 forwardRayOrigin, backRayOrigin, leftRayOrigin, rightRayOrigin;
    Vector3 backRayEndPoint, forwardRayEndPoint, leftRayEndPoint, rightRayEndPoint;
    
    [HideInInspector] public RaycastHit leftSlopeHit, rightSlopeHit, forwardSlopeHit, backSlopeHit;
    [HideInInspector] public RaycastHit forwardLeftSlopeHit, forwardRightSlopeHit, backLeftSlopeHit, backRightSlopeHit;
    
    //state machine
    public PlayerStateMachine stateMachine { get; private set; }
    //concrete states
    public PlayerSkatingState skatingState;
    public PlayerAirborneState airborneState;
    public PlayerHalfpipeState halfPipeState;
    public PlayerGrindState grindState;
    public PlayerDriftState driftState;
    public GameObject grindRailFollower;
    

    
    float jumpInput;

    private SlopeOrientationHandler slopeOrientationHandler;

#region Unity Abstracted Methods
    private void Awake()
    {
        StateMachineSetup();
        slopeOrientationHandler = new SlopeOrientationHandler(this, 
                                                                       playerData, 
                                                                       playerModelTransform, 
                                                                       inputTurningTransform);
    }
    
    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
        
        //Debug.Log(stateMachine.currentState);
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
        // Update the ray origin points
        UpdateRayOriginPoints();

        // Set Gizmos color
        Gizmos.color = Color.red;

        // Draw raycasts for CheckGround()
        Gizmos.DrawLine(backLeftRayOrigin, backLeftRayOrigin - playerModelTransform.up * playerData.slopeDownDetectionDistance);
        Gizmos.DrawLine(backRightRayOrigin, backRightRayOrigin - playerModelTransform.up * playerData.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardLeftRayOrigin, forwardLeftRayOrigin - playerModelTransform.up * playerData.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardRightRayOrigin, forwardRightRayOrigin - playerModelTransform.up * playerData.slopeDownDetectionDistance);
        
        Gizmos.color = Color.blue;
        

        // CheckGroundExtensions() rays (only for visuals in scene. Not used in actual code)
        Gizmos.DrawLine(backRayOrigin, backRayEndPoint);
        Gizmos.DrawLine(forwardRayOrigin, forwardRayEndPoint);
        Gizmos.DrawLine(leftRayOrigin, leftRayEndPoint);
        Gizmos.DrawLine(rightRayOrigin, rightRayEndPoint);

        // lines between the tips of each ground extension ray (makes a diamond around the player, this is whats used for edge detection)
        Gizmos.color = Color.green;
        
        Gizmos.DrawLine(forwardRayEndPoint, rightRayEndPoint);
        Gizmos.DrawLine(rightRayEndPoint, backRayEndPoint);
        Gizmos.DrawLine(backRayEndPoint, leftRayEndPoint);
        Gizmos.DrawLine(leftRayEndPoint, forwardRayEndPoint);
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

        bool isFacingUpward = angleWithDownward.IsInRangeOf(maxSlopeRange.x, maxSlopeRange.y);
        
        if (isFacingUpward) return;
        
        rb.AddForce(inputTurningTransform.forward * (movementSpeed * (InputRouting.Instance.GetMoveInput().y > 0 ? InputRouting.Instance.GetMoveInput().y : 0)), ForceMode.Acceleration); // Only adds force if
        // the player is not
        // on a slope that is
        // too steep.
    }

    public void OllieJump()
    {
        if (CheckGround())
        {
            rb.AddRelativeForce(transform.up * playerData.baseJumpForce, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// Handles turning the player model with left and right input. Rotating the player works best for the movement we
    /// are trying to achieve, as movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    public void TurnPlayer(bool overrideTurnSharpness = false, float newTurnSharpness = 0) // Rotates the PLAYER MODEL TRANSFORM. We must work with 2 transforms to achieve the desired effect.
    {
        inputTurningTransform.Rotate(0,
            overrideTurnSharpness ?
                newTurnSharpness * InputRouting.Instance.GetMoveInput().x :
                turnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime, 
            0, 
            Space.Self);
    }
    
    private void CalculateSpeedVector() 
        //TODO: The calculation in this method doesn't achieve desired results and can be improved.
    {
        float offset = rb.velocity.y;
        
        if (rb.velocity.y > 0)
        {
            offset = -rb.velocity.y * playerData.slopedUpSpeedMult;
        }
        else if (rb.velocity.y < 0)
        {
            offset = rb.velocity.y * playerData.slopedDownSpeedMult;
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
    
    /// <summary>
    /// De-accelerates the player by a fixed value. As long as the de-acceleration value is less than the acceleration
    /// value, the desired effect will work properly. Meant to be used in FixedUpdate.
    /// </summary>
    public void DeAccelerate() // Add Force feels too floaty, used on every frame to counteract the force.
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), playerData.deAccelerationSpeed);
    }


#endregion

#region Grinding Methods
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


#endregion

#region Helper Methods, Getters, & Setters

    public void SetRBKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }

    /// <summary>
    /// Updates every raycast origin point around the player whenever this is called.
    /// This must be called before doing any raycasts.
    /// </summary>
    private void UpdateRayOriginPoints()
    {
        Vector3 forwardMultByOffset = playerModelTransform.forward * playerData.slopeRayOffsetFromZ;
        Vector3 rightMultByOffset = playerModelTransform.right * playerData.slopeRayOffsetFromX;
        
        forwardLeftRayOrigin = raycastPoint.position + (forwardMultByOffset) - (rightMultByOffset);
        forwardRightRayOrigin = raycastPoint.position + (forwardMultByOffset) + (rightMultByOffset);
        
        backLeftRayOrigin = raycastPoint.position - (forwardMultByOffset) - (rightMultByOffset);
        backRightRayOrigin = raycastPoint.position - (forwardMultByOffset) + (rightMultByOffset);
        

        backRayOrigin = extensionRaycastPoint.position - forwardMultByOffset;
        forwardRayOrigin = extensionRaycastPoint.position + forwardMultByOffset;
        
        leftRayOrigin = extensionRaycastPoint.position - rightMultByOffset;
        rightRayOrigin = extensionRaycastPoint.position + rightMultByOffset;
        
        Vector3 forwardMultByDistance = inputTurningTransform.forward * playerData.slopeForwardDetectionDistance;
        Vector3 rightMultByDistance = inputTurningTransform.right * playerData.slopeForwardDetectionDistance;

        backRayEndPoint = backRayOrigin - forwardMultByDistance;
        forwardRayEndPoint = forwardRayOrigin + forwardMultByDistance;
        leftRayEndPoint = leftRayOrigin - rightMultByDistance;
        rightRayEndPoint = rightRayOrigin + rightMultByDistance;
        
    }
    
    public bool CheckGround()
    {
        var layerMask = (1 << LayerMask.NameToLayer("Ground"));
        
        UpdateRayOriginPoints();
        
        bool backLeftDownRayHit = Physics.Raycast(backLeftRayOrigin, -playerModelTransform.up, out backLeftSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        bool backRightDownRayHit = Physics.Raycast(backRightRayOrigin, -playerModelTransform.up, out backRightSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        
        bool forwardLeftDownRayHit = Physics.Raycast(forwardLeftRayOrigin, -playerModelTransform.up, out forwardLeftSlopeHit, playerData.slopeDownDetectionDistance, layerMask);
        bool forwardRightDownRayHit = Physics.Raycast(forwardRightRayOrigin, -playerModelTransform.up, out forwardRightSlopeHit, playerData.slopeDownDetectionDistance, layerMask);


        return (forwardLeftDownRayHit || forwardRightDownRayHit) || (backLeftDownRayHit || backRightDownRayHit);
    }

    /// <summary>
    /// This method is for checking a raycast forward from the skateboard, used for checking if the player
    /// has fallen horizontally near the ground
    /// </summary>
    /// <returns>True if Raycast hits ground</returns>
    public bool CheckGroundExtensions()
    {
        var layerMask = (1 << LayerMask.NameToLayer("Ground"));
        
        UpdateRayOriginPoints();
        
        
        bool lineOneHit = Physics.Linecast(forwardRayOrigin, rightRayEndPoint, out RaycastHit hit, layerMask);
        bool lineTwoHit = Physics.Linecast(rightRayOrigin, backRayEndPoint, out RaycastHit hit2, layerMask);
        bool lineThreeHit = Physics.Linecast(backRayOrigin, leftRayEndPoint, out RaycastHit hit3, layerMask);
        bool lineFourHit = Physics.Linecast(leftRayOrigin, forwardRayEndPoint, out RaycastHit hit4, layerMask);
        
        return lineOneHit || lineTwoHit || lineThreeHit || lineFourHit;
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }
    
    public SlopeOrientationHandler GetOrientationHandler()
    {
        return slopeOrientationHandler;
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
        stateMachine.Init(airborneState);
    }
    
}
