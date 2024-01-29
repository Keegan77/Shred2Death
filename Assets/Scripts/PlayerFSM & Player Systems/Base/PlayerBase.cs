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
    private PlayerStateMachine stateMachine;
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
        Gizmos.DrawLine(raycastPoint.position - transform.forward * playerData.slopeRayOffsetFromMid, leftSlopeHit.point);
        Gizmos.DrawLine(raycastPoint.position + transform.forward * playerData.slopeRayOffsetFromMid, rightSlopeHit.point);

    }
    
#endregion

#region Movement Methods
    public void SkateForward()
    {
        CalculateSpeedVector();
            
        float xRotation = TranslateEulersToRange180(transform.rotation.eulerAngles.x);
        float zRotation = TranslateEulersToRange180(transform.rotation.eulerAngles.z);


        if (rb.velocity.y > 0)
        {
            if (Mathf.Abs(xRotation) > playerData.slopeRangeWherePlayerCantMove.x &&
                Mathf.Abs(xRotation) < playerData.slopeRangeWherePlayerCantMove.y) return;
            if (Mathf.Abs(zRotation) > playerData.slopeRangeWherePlayerCantMove.x  &&
                Mathf.Abs(zRotation) < playerData.slopeRangeWherePlayerCantMove.y) return;
        } // If the player is on a slope that is too steep, don't add force
        
            
        rb.AddForce(inputTurningTransform.forward * (movementSpeed * (InputRouting.Instance.GetMoveInput().y > 0 ? InputRouting.Instance.GetMoveInput().y : 0)), ForceMode.Acceleration); // Only adds force if
        // the player is not
        // on a slope that is
        // too steep.
    }

    public void OllieJump()
    {
        Debug.Log("jump");
        JumpOffRail();
        if (CheckGround())
        {
            rb.AddRelativeForce(transform.up * playerData.baseJumpForce, ForceMode.Impulse);
        }
    }
    
    private void JumpOffRail()
    {
        if (stateMachine.currentState != grindState) return;
        var speed = GetComponent<SplineFollower>().followSpeed;
        SetRBKinematic(false);
        GameObject.Destroy(GetComponent<SplineFollower>());
        rb.AddForce(transform.forward * speed * 100);
        rb.AddForce(Vector3.up * 20);
        stateMachine.SwitchState(airborneState);
    }
    
    public void HalfPipeAirBehaviour()
    {
        Vector3 worldVelocity = rb.velocity;

        // converts the world velocity to local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
        
        localVelocity.y = 0;

        // converts the modified local velocity back to world space
        Vector3 newWorldVelocity = transform.TransformDirection(localVelocity);
        
        rb.velocity = newWorldVelocity;
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
    
    RaycastHit leftSlopeHit, rightSlopeHit;
    public void OrientToSlope()
    {
        if (CheckGround())
        {
            Vector3 averageNormal = (leftSlopeHit.normal + rightSlopeHit.normal).normalized;

            // stores perpendicular angle into targetRotation
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 1f);
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

#region Grinding

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
        Vector3 leftRayOrigin = raycastPoint.position - transform.forward * playerData.slopeRayOffsetFromMid;
        Vector3 rightRayOrigin = raycastPoint.position + transform.forward * playerData.slopeRayOffsetFromMid;
        bool leftHit = Physics.Raycast(leftRayOrigin, -transform.up, out leftSlopeHit, playerData.slopeDetectionDistance, 1 << LayerMask.NameToLayer("Ground"));
        bool rightHit = Physics.Raycast(rightRayOrigin, -transform.up, out rightSlopeHit, playerData.slopeDetectionDistance, 1 << LayerMask.NameToLayer("Ground"));
        
        return leftHit && rightHit;
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }


#endregion

#region Drifting


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
