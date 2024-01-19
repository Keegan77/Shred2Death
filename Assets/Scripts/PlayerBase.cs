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
    // Components
    private Rigidbody rb;
    [SerializeField] private Transform playerModel;
    //Input
    private Input input;
    private Vector2 moveInput;
    
    // Movement values
    [Header("Movement Values")]
    [SerializeField] float baseMovementSpeed;
    [Range(0, 1)]
    [SerializeField] float deAccelerationSpeed;
    [SerializeField] float turnSharpness;
    
    //Slope values
    [Header("Slope Values")]


    [SerializeField] private float orientToSlopeSpeed;
    [SerializeField] private float slopeDetectionDistance;

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

    private void FixedUpdate()
    {
        //SkateForward();
        OrientToSlope();
        //TurnPlayer();
        //DeAccelerate();
    }
    
    private void SkateForward() // Only accounts for flat ground movement for now.
    {
        rb.AddForce(playerModel.forward * (baseMovementSpeed * moveInput.y), ForceMode.Acceleration);
    }
    
    /// <summary>
    /// Handles turning the player. Rotating the player works best for the movement we are trying to achieve, as
    /// movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    private void TurnPlayer() // Rotates the PLAYER MODEL TRANSFORM. We must work with 2 transforms to achieve the desired effect.
    {
        //float turnAmount = moveInput.x * turnSharpness * Time.fixedDeltaTime;
        //Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + turnAmount, transform.eulerAngles.z);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
        
        if (moveInput.y != 0) playerModel.transform.Rotate(0, turnSharpness * moveInput.x * Time.fixedDeltaTime, 0, Space.Self);
        
    }
    
    private void OrientToSlope()
    {
        RaycastHit slopeHit;
        if (Physics.Raycast(transform.position,
                -transform.up,
                out slopeHit,
                slopeDetectionDistance,
                1 << LayerMask.NameToLayer("Ground")))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, slopeHit.normal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, slopeRotation, orientToSlopeSpeed * Time.fixedDeltaTime);
        }
    }
    
    
    /// <summary>
    /// De-accelerates the player by a fixed value. As long as the de-acceleration value is less than the acceleration
    /// value, the desired effect will work properly. Meant to be used in FixedUpdate.
    /// </summary>
    private void DeAccelerate() // Add Force feels too floaty!
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), deAccelerationSpeed);
    }
    
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - slopeDetectionDistance, transform.position.z));

    }
}
