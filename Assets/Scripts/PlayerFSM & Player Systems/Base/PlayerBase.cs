using System;
using System.Collections;
using Dreamteck.Splines;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class PlayerBase : MonoBehaviour
{
    #region Serialized Component References
        [Header("Private Component References")]
        [SerializeField] private Transform raycastPoint;
        [SerializeField] private Transform checkForBowlRaycastPoint;
        [SerializeField] private Transform extensionRaycastPoint;
        [SerializeField] private Transform chestPivot, originPivot;
        private SlopeOrientationHandler orientationHandler;
        private TrickComboHandler comboHandler;
        public PlayerHUD playerHUD;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private Camera cam;
        private PlayerRagdollHandler ragdollHandler;
        
    #endregion

    #region Public Component References
        [Header("Public Component References")]
        public PlayerParticleManager particleManager;
        public GunfireHandler gunfireHandler;
        public GunSwitcher gunSwitcher;
        public PlayerRootMover rootMover;
        public PlayerCapsuleFloater capsuleFloater;
        public Rigidbody rb;
        public Transform inputTurningTransform, playerModelTransform; // this is public because we want access from our states
        [Tooltip("Holds all of the player's base movement values.")]
        public PlayerData playerData;

        public RigWeightController proceduralRigController;
        public GameObject shotgunUltSpiralTrail;
        public GameObject shotgunUltSelectionCircle;

        public bool grindSpeedOverride;
        public float overrideSpeed;
        
    #endregion

    #region Class fields

    private bool timerRanOut;
        private SplineComputer currentSpline;
        private double splineCompletionPercent;
        public PlayerMovementMethods movement { get; private set; }
        //public ConstantForce constantForce;
        public RaycastHit forwardLeftSlopeHit, forwardRightSlopeHit, backLeftSlopeHit, backRightSlopeHit;

        private bool queueJump;
        
        [HideInInspector]
        public Vector3 forwardLeftRayOrigin, forwardRightRayOrigin, backLeftRayOrigin, backRightRayOrigin;
        [HideInInspector] 
        public Vector3 forwardRayOrigin, backRayOrigin, leftRayOrigin, rightRayOrigin;
        Vector3 backRayEndPoint, forwardRayEndPoint, leftRayEndPoint, rightRayEndPoint;
        
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
        public PlayerDeathState deathState;
        public PlayerDropinState dropinState;
        
        public AbilityStateMachine abilityStateMachine { get; private set; }
        public IntermediaryAbilityState intermediaryAbilityState;
        public DualieUltimateAbilityState dualieUltimateAbilityState;
        public ShotgunUltimateAbilityState shotgunUltimateAbilityState;
        public BoostAbilityState boostAbilityState;
        
        
        
        public GameObject grindRailFollower;
        
    #endregion

    #region Unity Methods
    private void Awake()
    {
        StateMachineSetup();
        orientationHandler = GetComponent<SlopeOrientationHandler>();
        comboHandler = GetComponent<TrickComboHandler>();
        ragdollHandler = GetComponent<PlayerRagdollHandler>();
        particleManager = GetComponent<PlayerParticleManager>();
        capsuleFloater = GetComponent<PlayerCapsuleFloater>();
        
        movement = new PlayerMovementMethods(this, rb, playerData, inputTurningTransform);
    }
    
    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
        abilityStateMachine.currentAbilityState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
        abilityStateMachine.currentAbilityState.PhysicsUpdate();
    } 
    private void OnTriggerEnter(Collider other)
    {
        stateMachine.currentState.StateTriggerEnter(other);
        
        if (other.CompareTag("Ramp90"))
        {
            orientationHandler.ChangePivot(transform, chestPivot.position);
            //playerData.grindPositioningOffset = 3.38f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        stateMachine.currentState.StateTriggerExit(other);
        if (other.CompareTag("Ramp90"))
        {
            orientationHandler.ChangePivot(transform, originPivot.position);
            //playerData.grindPositioningOffset = .2f;
        }
    }
    
    
    private void OnTriggerStay(Collider other) => stateMachine.currentState.StateTriggerStay(other);


    private void OnEnable()
    {
        InputRouting.Instance.input.UI.Pause.performed += ctx =>
        {
            if (!timerRanOut && !health.IsDead()) playerHUD.ToggleGamePaused();
        };
    }

    private void OnDisable()
    {
        InputRouting.Instance.input.UI.Pause.performed -= ctx =>
        {
            if (!timerRanOut) playerHUD.ToggleGamePaused();
        };
    }
    

    private void OnCollisionEnter(Collision other)
    {
        stateMachine.currentState.StateCollisionEnter(other);
        if (other.gameObject.CompareTag("BurnDamageFireWall"))
        {
            movement.DoBurnForce(other.contacts[0].point, 10);
        }
    }

    private void OnDrawGizmos()
    {
        // Update the ray origin points
        //UpdateRayOriginPoints();

        
        
        // Set Gizmos color
        Gizmos.color = Color.red;

        // Draw raycasts for CheckGround()
        /*Gizmos.DrawLine(backLeftRayOrigin, backLeftRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(backRightRayOrigin, backRightRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardLeftRayOrigin, forwardLeftRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);
        Gizmos.DrawLine(forwardRightRayOrigin, forwardRightRayOrigin - playerModelTransform.up * orientationHandler.slopeDownDetectionDistance);*/
        
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

        Vector3 newBackLeft = new Vector3(backLeftRayOrigin.x, backLeftRayOrigin.y, backLeftRayOrigin.z);
        Vector3 newBackRight = new Vector3(backRightRayOrigin.x, backRightRayOrigin.y, backRightRayOrigin.z);
        Vector3 rayOrigin = (newBackLeft + newBackRight) / 2;
        //convert ray origin to local space from player
        Vector3 rayOriginLocal = transform.InverseTransformPoint(rayOrigin);
        rayOriginLocal.z -= 1f;
        rayOrigin = transform.TransformPoint(rayOriginLocal);
        Vector3 rayDirection = -transform.up;
        float rayLength = 4;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin, rayDirection * rayLength);
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

    public RaycastHit[] ReturnSplineDetection()
    {
        return Physics.SphereCastAll(transform.position,
                                          playerData.grindDetectionRadius,
                                   transform.forward,
                                0,
                                  1 << LayerMask.NameToLayer("Spline"));
    }


    private bool ran;

    /// <summary>
    /// We reset the bool with this because we get janky movement if the bool isnt set to false after detection is false
    /// </summary>
    public void ResetGrindUI()
    {
        ran = false;
    }
    
    public void UpdateGrindRailUI()
    {
        GrindButtonBehaviour grindButton = playerHUD.grindDisplayButton.GetComponent<GrindButtonBehaviour>();
        if (ReturnSplineDetection().Length != 0)
        {
            Vector3 cachedPos = ReturnSplineDetection()[0].transform.GetComponent<SplineComputer>()
                .Project(transform.position).position; //insane line of code lmao
            if (!ran)
            {
                grindButton.SetCurrentPosition(cachedPos + Vector3.up * 2f);
                ran = true;
            }
            grindButton.SetSpringyScale(grindButton.maxUniformScale);
            grindButton.SetSpringyPosition(cachedPos + Vector3.up * 2f);
        }
        else
        {
            ran = false;
            DisableGrindRailUI();
        }
    }
    
    public void DisableGrindRailUI()
    {
        GrindButtonBehaviour button = playerHUD.grindDisplayButton.GetComponent<GrindButtonBehaviour>();
        button.SetSpringyScale(button.minUniformScale);
    }
    
    public void CheckAndSetSpline()
    {
        RaycastHit[] hits = ReturnSplineDetection();
        Debug.Log("checked");
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
    
    public void OverrideGrindSpeed(float speed)
    {
        overrideSpeed = speed;
        grindSpeedOverride = true;
    }


#endregion

    #region Helper Methods, Getters, & Setters
    
    public PlayerRagdollHandler GetRagdollHandler()
    {
        return ragdollHandler;
    }

    public bool JumpQueued()
    {
        return queueJump;
    }
    
    public void SetJumpQueued(bool value)
    {
        queueJump = value;
    }
    
    public Camera GetPlayerCamera()
    {
        return cam;
    }
    
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
        

        //extension raycasts
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
        var layerMask = 1 << LayerMask.NameToLayer(layerName);
        var layerMask2 = 1 << LayerMask.NameToLayer("Default");
        var combinedLayerMask = layerMask | layerMask2;
        UpdateRayOriginPoints();

        bool backLeftDownRayHit = Physics.Raycast(backLeftRayOrigin, -playerModelTransform.up, out backLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, combinedLayerMask);
        bool backRightDownRayHit = Physics.Raycast(backRightRayOrigin, -playerModelTransform.up, out backRightSlopeHit, orientationHandler.slopeDownDetectionDistance, combinedLayerMask);
        
        bool forwardLeftDownRayHit = Physics.Raycast(forwardLeftRayOrigin, -playerModelTransform.up, out forwardLeftSlopeHit, orientationHandler.slopeDownDetectionDistance, combinedLayerMask);
        bool forwardRightDownRayHit = Physics.Raycast(forwardRightRayOrigin, -playerModelTransform.up, out forwardRightSlopeHit, orientationHandler.slopeDownDetectionDistance, combinedLayerMask);

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
    public float GetRightAngleWithDownward()
    {
        return Vector3.Angle(transform.right, Vector3.down);
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
        deathState = new PlayerDeathState(this, stateMachine);
        dropinState = new PlayerDropinState(this, stateMachine);
        stateMachine.Init(airborneState);
        
        abilityStateMachine = new AbilityStateMachine();
        intermediaryAbilityState = new IntermediaryAbilityState(this, abilityStateMachine);
        dualieUltimateAbilityState = new DualieUltimateAbilityState(this, abilityStateMachine);
        shotgunUltimateAbilityState = new ShotgunUltimateAbilityState(this, abilityStateMachine);
        boostAbilityState = new BoostAbilityState(this, abilityStateMachine);
        abilityStateMachine.Init(intermediaryAbilityState);
        
        
    }
    
}
