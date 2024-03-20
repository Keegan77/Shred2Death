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
    public float turnSharpness { get; private set; }
    private float boostTimer;
    private bool burnCooldownActive;

    private float baseSpeed;
    public PlayerMovementMethods(PlayerBase player, Rigidbody rb, PlayerData playerData, Transform inputTurningTransform)
    {
        this.rb = rb;
        this.playerData = playerData;
        this.player = player;
        this.inputTurningTransform = inputTurningTransform;
        
        baseSpeed = playerData.baseMovementSpeed;
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
        
        
        Quaternion localTargetRotation = Quaternion.Inverse(player.transform.rotation) * player.GetOrientationHandler().targetRotation;
        // we inverse the player's rotation to get the rotation difference between the player's current rotation and the target rotation
        
        
        // Use localTargetRotation to get the local rotation and use forward to set the direction
        Vector3 forwardAfterRotation = localTargetRotation * inputTurningTransform.forward;
        Debug.DrawRay(player.transform.position, forwardAfterRotation, Color.magenta);
        
        
        // Apply force in the direction of forwardAfterRotation
        rb.AddForce(forwardAfterRotation * (movementSpeed * (InputRouting.Instance.GetMoveInput().y > 0.25f ? 
            1 : 0)), ForceMode.Acceleration);
        
        
    }

    public void OllieJump()
    {
        if (player != null) rb.AddRelativeForce(player.transform.up * playerData.baseJumpForce, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Handles turning the player model with left and right input. Rotating the player works best for the movement we
    /// are trying to achieve, as movement is based on the player's forward direction. Meant to be used in FixedUpdate.
    /// </summary>
    public void TurnPlayer(bool overrideTurnSharpness = false, float newTurnSharpness = 0) // Rotates the input turning transform
    {
        if (overrideTurnSharpness)
        {
            player.inputTurningTransform.Rotate(0,
                newTurnSharpness * InputRouting.Instance.GetMoveInput().x, 
                0, Space.Self);
        }
        else
        {
            player.transform.Rotate(0,
                turnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime, 
                0, Space.Self);
        }
        
        
    }

    public void DoBurnForce(Vector3 contactPoint, float dmg)
    {
        if (burnCooldownActive) return;
        Debug.Log("burn dmg");
        
        player.GetComponentInChildren<IDamageable>()?.TakeDamage(dmg);
        
        player.StartCoroutine(BurnForceTimer());
        
        // Calculate the direction from the player to the point of collision
        Vector3 collisionDirection = contactPoint - player.transform.position;
        //transform.rotation = Quaternion.LookRotation(new Vector3(transform.rotation.x, collisionDirection.y, transform.rotation.z));

        // Normalize the direction
        collisionDirection = collisionDirection.normalized;
        Vector3 force = new Vector3(collisionDirection.x, -collisionDirection.y, collisionDirection.z);

        // Apply a force in the opposite direction of the collision
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(-collisionDirection.x, playerData.extraBurnVerticalForce , -collisionDirection.z) * playerData.burnForce, ForceMode.Impulse);
    }
    
    private IEnumerator BurnForceTimer()
    {
        burnCooldownActive = true;
        yield return new WaitForSeconds(playerData.burnBounceCooldown);
        burnCooldownActive = false;
    }
    
    private void CalculateCurrentSpeed() 
    {
        float offset = rb.velocity.y;
        float extraForce;
        Func<float, float> calculateExtraForce = (slopeMultiplier) =>
            -(player.GetOrientationWithDownward() - 90) * slopeMultiplier; // this is a negative so if we are going
                                                                           // down, we add force, if we are going up,
                                                                           // we decrease force
        if (rb.velocity.y > 0)
        {
            offset = calculateExtraForce(playerData.slopedUpSpeedMult);
        }
        else if (rb.velocity.y < 0)
        {
            offset = calculateExtraForce(playerData.slopedDownSpeedMult);
        }
        // Get the rotation around the x-axis, ranging from -90 to 90
        
        movementSpeed = baseSpeed + offset;
        //Debug.Log(movementSpeed);
    }

    public void CalculateTurnSharpness()
    {
        turnSharpness = playerData.baseTurnSharpness;
        //if (rb.velocity.magnitude < 20) turnSharpness = playerData.baseTurnSharpness;
        //else turnSharpness = playerData.baseTurnSharpness / (rb.velocity.magnitude / 2);
    }
    
    /// <summary>
    /// De-accelerates the player by a fixed value. As long as the de-acceleration value is less than the acceleration
    /// value, the desired effect will work properly. Meant to be used in FixedUpdate.
    /// </summary>
    public void DeAccelerate() // Add Force feels too floaty, used on every frame to counteract the force.
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), playerData.deAccelerationSpeed);
    }
    
    private Coroutine boostTimerCoroutine;
    private Coroutine rechargeBoostCoroutine;
    public void StartBoost() // subscribe to on input performed boost input
    {
        if (boostTimer > playerData.boostDuration) return;
        if (rechargeBoostCoroutine != null)
        {
            player.StopCoroutine(rechargeBoostCoroutine);
            currentlyRecharging = false;
            rechargeBoostCoroutine = null;
        }
        
        baseSpeed = playerData.baseBoostSpeed;
        if (currentlyBoosting) return;
        boostTimerCoroutine = player.StartCoroutine(BoostTimer());
    }

    bool currentlyBoosting;
    bool currentlyRecharging;
    public void StopBoost() // subscribe this to on input canceled boost input cancel
    {
        if (boostTimerCoroutine != null)
        {
            player.StopCoroutine(boostTimerCoroutine);
            currentlyBoosting = false;
            boostTimerCoroutine = null;
        }
        baseSpeed = playerData.baseMovementSpeed;
        if (currentlyRecharging) return;
        rechargeBoostCoroutine = player.StartCoroutine(RechargeBoost());
    }
    
    private IEnumerator BoostTimer()
    {
        currentlyBoosting = true;
        while (boostTimer < playerData.boostDuration)
        {
            boostTimer += Time.deltaTime;
            yield return null;
        }
        currentlyBoosting = false;
        StopBoost();
    }

    private IEnumerator RechargeBoost()
    {
        currentlyRecharging = true;
        while (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            yield return null;
        }
        currentlyRecharging = false;
    }
    
}
