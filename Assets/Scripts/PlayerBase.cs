using System;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;

//README
// This script affects rotation values for two transforms. The parent transform is used for slope orientation, while
// the child transform is used for turning the player + the player's correct forward direction, which is used for the forward force,
// which is applied to the RB. as affected by the parent transform. This is done to achieve the desired effect of the player model
// rotating perpendicularly to the slope, while the player's forward direction is based on the parent transform's
// forward direction.


public class PlayerBase : MonoBehaviour
{
    private enum PlayerState // simple shitty state machine
    {
        Skating,
        Airborne
    }

    private PlayerState currentState;

    // Components
    private Rigidbody rb;
    [SerializeField] private Transform playerModel;
    //Input
    private Input input;
    private Vector2 moveInput;
    
    // Movement values
    [Header("Movement Values")]
    [SerializeField] float baseMovementSpeed;

    private float movementSpeed;
    [Range(0, 1)]
    [SerializeField] float deAccelerationSpeed;
    [SerializeField] float turnSharpness;
    
    //Slope values
    [Header("Slope Values")]
    
    [SerializeField] private float orientToSlopeSpeed;
    [SerializeField] private float slopeDetectionDistance;
    [Tooltip("The distance from the center of the player to the left and right raycast origins. These are used to detect the slope.")]
    [SerializeField] private float slopeRayOffsetFromMid;
    [Tooltip("X is min, Y is max. If the slope is within this range, the player will not be able to exert a forward force. Used for preventing the player from using forward force up slopes that are too steep")]
    [SerializeField] private Vector2 slopeRangeWherePlayerCantMove;

    [SerializeField] private float slopedUpSpeedMultipler, slopedDownSpeedMultipler;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform boxPos;
    [SerializeField] private Vector3 boxSize;
    
    //Half Pipe values
    private bool onHalfPipeRamp;

    private void Awake()
    {
        input = new Input();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        
    }
    
    private void CalculateSpeedVector()
    {
        float offset = rb.velocity.y;
        
        if (rb.velocity.y > 0)
        {
            offset = -rb.velocity.y * slopedUpSpeedMultipler;
        }
        else if (rb.velocity.y < 0)
        {
            offset = rb.velocity.y / slopedDownSpeedMultipler;
        }
        // Get the rotation around the x-axis, ranging from -90 to 90
        
        movementSpeed = baseMovementSpeed + offset;
        //Debug.Log(movementSpeed);
    }
    
    private void FixedUpdate()
    {
        if (CheckGround())
        {
            SkateForward(); 
            DeAccelerate();
            OrientToSlope();
            currentState = PlayerState.Skating;
        }
        else
        {
            currentState = PlayerState.Airborne;
            if (onHalfPipeRamp) HalfPipeAirBehaviour();
            else ReOrient();
        }
        TurnPlayer();

    }

    private void HalfPipeAirBehaviour()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
    
    private void SkateForward()
    {
        CalculateSpeedVector();
        
        float xRotation = TranslateEulersToRange180(transform.rotation.eulerAngles.x);
        float zRotation = TranslateEulersToRange180(transform.rotation.eulerAngles.z);
        
        
        if (Mathf.Abs(xRotation) > slopeRangeWherePlayerCantMove.x &&
            Mathf.Abs(xRotation) < slopeRangeWherePlayerCantMove.y) return;
        if (Mathf.Abs(zRotation) > slopeRangeWherePlayerCantMove.x  &&
            Mathf.Abs(zRotation) < slopeRangeWherePlayerCantMove.y) return;
        
        rb.AddForce(playerModel.forward * (movementSpeed * moveInput.y), ForceMode.Acceleration); // Only adds force if
                                                                                                  // the player is not
                                                                                                  // on a slope that is
                                                                                                  // too steep.
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
    
    /// <summary>
    /// Handles turning the player model with left and right input. Rotating the player works best for the movement we
    /// are trying to achieve, as movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    private void TurnPlayer() // Rotates the PLAYER MODEL TRANSFORM. We must work with 2 transforms to achieve the desired effect.
    {
        if (moveInput.y != 0 || currentState == PlayerState.Airborne) playerModel.transform.Rotate(0, turnSharpness * moveInput.x * Time.fixedDeltaTime, 0, Space.Self);
        
    }

    RaycastHit leftSlopeHit, rightSlopeHit;
    private void OrientToSlope()
    {
        // Define points on either side of the skateboard
        Vector3 leftRayOrigin = transform.position - transform.forward * slopeRayOffsetFromMid;
        Vector3 rightRayOrigin = transform.position + transform.forward * slopeRayOffsetFromMid;

        // Perform raycasts from the defined points
        bool leftHit = Physics.Raycast(leftRayOrigin, -transform.up, out leftSlopeHit, slopeDetectionDistance, 1 << LayerMask.NameToLayer("Ground"));
        bool rightHit = Physics.Raycast(rightRayOrigin, -transform.up, out rightSlopeHit, slopeDetectionDistance, 1 << LayerMask.NameToLayer("Ground"));
        
        if (leftHit && rightHit)
        {
            // Calculate the average normal
            Vector3 averageNormal = (leftSlopeHit.normal + rightSlopeHit.normal).normalized;

            // Calculate the desired rotation
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
            
            // Lerp to the desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, Time.fixedDeltaTime * orientToSlopeSpeed);
        }
    }

    /// <summary>
    /// Slowly re-orients the player mid-air to be upright. Meant to be used in FixedUpdate.
    /// </summary>
    private void ReOrient()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 1f);
    }
    
    /// <summary>
    /// De-accelerates the player by a fixed value. As long as the de-acceleration value is less than the acceleration
    /// value, the desired effect will work properly. Meant to be used in FixedUpdate.
    /// </summary>
    private void DeAccelerate() // Add Force feels too floaty!
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), deAccelerationSpeed);
    }

    private bool CheckGround()
    {
        return Physics.CheckBox(boxPos.position, boxSize, transform.rotation, 1 << LayerMask.NameToLayer("Ground"));
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.forward * slopeRayOffsetFromMid, leftSlopeHit.point);
        Gizmos.DrawLine(transform.position + transform.forward * slopeRayOffsetFromMid, rightSlopeHit.point);
        Gizmos.DrawWireCube(boxPos.position, boxSize/2);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ramp90")) onHalfPipeRamp = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ramp90")) onHalfPipeRamp = false;
    }
}
