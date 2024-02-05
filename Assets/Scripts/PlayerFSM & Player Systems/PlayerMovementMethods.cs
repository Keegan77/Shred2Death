using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementMethods
{
    private Rigidbody rb;
    private PlayerData playerData;
    private PlayerBase player;
    private Transform inputTurningTransform;
    private float movementSpeed;
    private float turnSharpness;
    public PlayerMovementMethods(PlayerBase player, Rigidbody rb, PlayerData playerData, Transform inputTurningTransform)
    {
        this.rb = rb;
        this.playerData = playerData;
        this.player = player;
        this.inputTurningTransform = inputTurningTransform;
    }
    
    /// <summary>
    /// Will exert a force forward if the player's slope isn't too steep. Meant to be used in FixedUpdate.
    /// </summary>
    public void SkateForward()
    {
        CalculateCurrentSpeed();
        
        Vector2 maxSlopeRange = new Vector2(playerData.slopeRangeWherePlayerCantMove.x + 90, playerData.slopeRangeWherePlayerCantMove.y + 90);
        
        // calculates the angle between the player's forward direction and the world's down direction
        float angleWithDownward = player.GetOrientationWithDownward();

        //Debug.Log(angleWithDownward);

        bool isFacingUpward = angleWithDownward.IsInRangeOf(maxSlopeRange.x, maxSlopeRange.y);
        
        if (isFacingUpward) return;
        
        rb.AddForce(inputTurningTransform.forward * (movementSpeed * (InputRouting.Instance.GetMoveInput().y > 0 ? 
            InputRouting.Instance.GetMoveInput().y : 0)), ForceMode.Acceleration); 
        // Only adds force if the player is not on a slope that is too steep.
    }

    public void OllieJump()
    {
        rb.AddRelativeForce(player.transform.up * playerData.baseJumpForce, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Handles turning the player model with left and right input. Rotating the player works best for the movement we
    /// are trying to achieve, as movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    public void TurnPlayer(bool overrideTurnSharpness = false, float newTurnSharpness = 0) // Rotates the input turning transform
    {
        inputTurningTransform.Rotate(0,
            overrideTurnSharpness ?
                newTurnSharpness * InputRouting.Instance.GetMoveInput().x :
                turnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime, 
            0, 
            Space.Self);
    }
    
    private void CalculateCurrentSpeed() 
    {
        float offset = rb.velocity.y;
        float extraForce;
        Func<float, float> calculateExtraForce = (slopeMultiplier) =>
            -(player.GetOrientationWithDownward() - 90) * slopeMultiplier; // this is a negative so if we are going
                                                                           // down, we add force, if we are going up,
                                                                           // we decrease force
        
        Debug.Log(calculateExtraForce(playerData.slopedUpSpeedMult));
        if (rb.velocity.y > 0)
        {
            offset = calculateExtraForce(playerData.slopedUpSpeedMult);
        }
        else if (rb.velocity.y < 0)
        {
            offset = calculateExtraForce(playerData.slopedDownSpeedMult);
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
}
