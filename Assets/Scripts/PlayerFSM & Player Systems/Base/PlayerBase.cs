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
    [SerializeField] private SlopeOrientationHandler orientationHandler;
    private SplineComputer currentSpline;
    private double splineCompletionPercent;
    private PlayerMovementMethods movementMethods;
    
    //Player Data
    [Header("Player Data")] [Tooltip("Holds all of the player's base movement values.")]
    public PlayerData playerData;
    //Variables which hold calculated values based on their base constants.

    
    //raycast slope detection origin points
    [HideInInspector] public Vector3 forwardLeftRayOrigin, forwardRightRayOrigin, backLeftRayOrigin, backRightRayOrigin;
    [HideInInspector] public Vector3 forwardRayOrigin, backRayOrigin, leftRayOrigin, rightRayOrigin;
    Vector3 backRayEndPoint, forwardRayEndPoint, leftRayEndPoint, rightRayEndPoint;
    
    //[HideInInspector] public RaycastHit leftSlopeHit, rightSlopeHit, forwardSlopeHit, backSlopeHit;
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
    

#region Unity Methods
    private void Awake()
    {
        StateMachineSetup();
        movementMethods = new PlayerMovementMethods(this, rb, playerData, inputTurningTransform);
    }
    
    private void Update() => stateMachine.currentState.LogicUpdate();
    private void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();
    private void OnTriggerEnter(Collider other) => stateMachine.currentState.StateTriggerEnter(other);
    private void OnTriggerStay(Collider other) => stateMachine.currentState.StateTriggerStay(other);
    private void OnTriggerExit(Collider other) => stateMachine.currentState.StateTriggerExit(other);
    private void OnCollisionStay(Collision other) => stateMachine.currentState.StateCollisionEnter(other);
    private void OnDrawGizmos()
    {
        // Update the ray origin points
        UpdateRayOriginPoints();

        // Set Gizmos color
        Gizmos.color = Color.red;

        // Draw raycasts for CheckGround()
        Gizmos.DrawLine(backLeftRayOrigin, backLeftRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(backRightRayOrigin, backRightRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardLeftRayOrigin, forwardLeftRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardRightRayOrigin, forwardRightRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        
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

#region Movement Class Getter

    public PlayerMovementMethods GetMovementMethods()
    {
        return movementMethods;
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
        Vector3 forwardMultByOffset = playerModelTransform.forward * orientationHandler.slopeRayOffsetFromZ;
        Vector3 rightMultByOffset = playerModelTransform.right * orientationHandler.slopeRayOffsetFromX;
        
        forwardLeftRayOrigin = raycastPoint.position + (forwardMultByOffset) - (rightMultByOffset);
        forwardRightRayOrigin = raycastPoint.position + (forwardMultByOffset) + (rightMultByOffset);
        
        backLeftRayOrigin = raycastPoint.position - (forwardMultByOffset) - (rightMultByOffset);
        backRightRayOrigin = raycastPoint.position - (forwardMultByOffset) + (rightMultByOffset);
        

        backRayOrigin = extensionRaycastPoint.position - forwardMultByOffset;
        forwardRayOrigin = extensionRaycastPoint.position + forwardMultByOffset;
        
        leftRayOrigin = extensionRaycastPoint.position - rightMultByOffset;
        rightRayOrigin = extensionRaycastPoint.position + rightMultByOffset;
        
        Vector3 forwardMultByDistance = inputTurningTransform.forward * orientationHandler.slopeForwardDetectionDistance;
        Vector3 rightMultByDistance = inputTurningTransform.right * orientationHandler.slopeForwardDetectionDistance;

        backRayEndPoint = backRayOrigin - forwardMultByDistance;
        forwardRayEndPoint = forwardRayOrigin + forwardMultByDistance;
        leftRayEndPoint = leftRayOrigin - rightMultByDistance;
        rightRayEndPoint = rightRayOrigin + rightMultByDistance;
        
    }
    
    public bool CheckGround()
    {
        var layerMask = (1 << LayerMask.NameToLayer("Ground"));
        
        UpdateRayOriginPoints();
        
        bool backLeftDownRayHit = Physics.Raycast(backLeftRayOrigin, -playerModelTransform.up, out backLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);
        bool backRightDownRayHit = Physics.Raycast(backRightRayOrigin, -playerModelTransform.up, out backRightSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);
        
        bool forwardLeftDownRayHit = Physics.Raycast(forwardLeftRayOrigin, -playerModelTransform.up, out forwardLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);
        bool forwardRightDownRayHit = Physics.Raycast(forwardRightRayOrigin, -playerModelTransform.up, out forwardRightSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);


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
        return orientationHandler;
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
