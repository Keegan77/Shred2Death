using System;
using System.Collections;
using Dreamteck.Splines;
using UnityEngine.InputSystem;
using UnityEngine;

[SelectionBase]
public class PlayerBase : MonoBehaviour
{
    #region Serialized Component References
        [Header("Private Component References")]
        [SerializeField] private Transform raycastPoint;
        [SerializeField] private Transform checkForBowlRaycastPoint;
        [SerializeField] private Transform extensionRaycastPoint;
        [SerializeField] private Transform chestPivot, originPivot;
        [SerializeField] private SlopeOrientationHandler orientationHandler;
        [SerializeField] private CapsuleCollider skateboardColliderCapsule;
        [SerializeField] private TrickComboHandler comboHandler;
    #endregion

    #region Public Component References
        [Header("Public Component References")]
        public Rigidbody rb;
        public Transform inputTurningTransform, playerModelTransform; // this is public because we want access from our states
        [Tooltip("Holds all of the player's base movement values.")]
        public PlayerData playerData;
        
    #endregion

    #region Private class fields
        private SplineComputer currentSpline;
        private double splineCompletionPercent;
        public PlayerMovementMethods movement { get; private set; }
        public ConstantForce constantForce;
        public RaycastHit forwardLeftSlopeHit, forwardRightSlopeHit, backLeftSlopeHit, backRightSlopeHit;
        [HideInInspector] 
        public Vector3 forwardLeftRayOrigin, forwardRightRayOrigin, backLeftRayOrigin, backRightRayOrigin;
        [HideInInspector] 
        public Vector3 forwardRayOrigin, backRayOrigin, leftRayOrigin, rightRayOrigin;
        Vector3 backRayEndPoint, forwardRayEndPoint, leftRayEndPoint, rightRayEndPoint;
        
        float skateboardOriginalColliderRadius;
    #endregion

    #region State Factory
        public PlayerStateMachine stateMachine { get; private set; }
        //concrete states
        public PlayerSkatingState skatingState;
        public PlayerAirborneState airborneState;
        public PlayerHalfpipeState halfPipeState;
        public PlayerGrindState grindState;
        public PlayerDriftState driftState;
        public PlayerNosediveState nosediveState;
        public PlayerDropinState dropinState;
        public GameObject grindRailFollower;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        StateMachineSetup();
        skateboardOriginalColliderRadius = skateboardColliderCapsule.radius;
        movement = new PlayerMovementMethods(this, rb, playerData, inputTurningTransform);
    }
    
    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
        //Debug.Log(GetOrientationWithDownward()); 
        //Debug.Log(GetOrientationWithDownward() - 90);
    } 
    
    private void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();
    private void OnTriggerEnter(Collider other)
    {
        stateMachine.currentState.StateTriggerEnter(other);

        if (other.CompareTag("Ramp90"))
        {
            orientationHandler.ChangePivot(transform, chestPivot.position);
            playerData.grindPositioningOffset = 3.38f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        stateMachine.currentState.StateTriggerExit(other);
        if (other.CompareTag("Ramp90"))
        {
            orientationHandler.ChangePivot(transform, originPivot.position);
            playerData.grindPositioningOffset = .2f;
        }
    }
    
    
    private void OnTriggerStay(Collider other) => stateMachine.currentState.StateTriggerStay(other);


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
        
        Gizmos.color = Color.yellow;
        RaycastHit hit = RaycastFromBowlCheckPoint();
        
        if (hit.collider != null)
        {
            Gizmos.DrawLine(checkForBowlRaycastPoint.position, hit.point);
        }
        else
        {
            Gizmos.DrawLine(checkForBowlRaycastPoint.position, checkForBowlRaycastPoint.position - transform.forward * 10f);
        }
    }
    
#endregion

    #region Movement Class Getter

    public PlayerMovementMethods GetMovementMethods()
    {
        return movement;
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
    }
    
    public void CheckAndSetSpline()
    {
        float radius = 10f;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, 0, 1 << LayerMask.NameToLayer("Spline"));
        
        foreach (RaycastHit hit in hits)
        {
            SplineComputer hitSpline = hit.collider.GetComponent<SplineComputer>();
            SplineSample hitPoint = hitSpline.Project(transform.position);
            //Debug.Log(Vector3.Distance(player.transform.position, hitPoint.position));
            if (Vector3.Distance(transform.position, hitPoint.position) < playerData.railSnapDistance)
            {
                SetCurrentSpline(hitSpline, hitPoint);
                stateMachine.SwitchState(grindState);
            }
        }
    }


#endregion

    #region Helper Methods, Getters, & Setters
    
    public TrickComboHandler GetComboHandler()
    {
        return comboHandler;
    }

    public void SetRBKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }

    public RaycastHit RaycastFromBowlCheckPoint()
    {
        Physics.Raycast(checkForBowlRaycastPoint.position, -transform.forward, out RaycastHit hit, 10f);
        return hit;
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
    
    public bool CheckGround(string layerName = "Ground")
    {

        var layerMask = (1 << LayerMask.NameToLayer(layerName));
        UpdateRayOriginPoints();

        bool backLeftDownRayHit = Physics.Raycast(backLeftRayOrigin, -playerModelTransform.up, out backLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);
        bool backRightDownRayHit = Physics.Raycast(backRightRayOrigin, -playerModelTransform.up, out backRightSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);

        bool forwardLeftDownRayHit = Physics.Raycast(forwardLeftRayOrigin, -playerModelTransform.up, out forwardLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);
        bool forwardRightDownRayHit = Physics.Raycast(forwardRightRayOrigin, -playerModelTransform.up, out forwardRightSlopeHit, orientationHandler.slopeDownDetectionDistance, layerMask);

        return (forwardLeftDownRayHit && forwardRightDownRayHit) || (backLeftDownRayHit && backRightDownRayHit);
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
        return Vector3.Angle(transform.forward, Vector3.down);
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
        nosediveState = new PlayerNosediveState(this, stateMachine);
        dropinState = new PlayerDropinState(this, stateMachine);
        stateMachine.Init(airborneState);
    }
    
    public float GetOriginalColliderRadius()
    {
        return skateboardOriginalColliderRadius;
    }
    public IEnumerator ScaleCapsuleCollider(float newRadius)
    {
        float duration = .15f; // Duration for the scaling operation, adjust as needed
        float elapsed = 0f;

        float originalRadius = skateboardColliderCapsule.radius;

        while (Mathf.Abs(skateboardColliderCapsule.radius - newRadius) > 0.01f)
        {
            elapsed += Time.deltaTime;
            skateboardColliderCapsule.radius = Mathf.Lerp(originalRadius, newRadius, elapsed / duration);
            yield return null;
        }

        // Ensure the final radius is exactly what's expected
        skateboardColliderCapsule.radius = newRadius;
    }
    
}
