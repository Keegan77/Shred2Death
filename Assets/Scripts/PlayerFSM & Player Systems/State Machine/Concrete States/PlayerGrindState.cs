using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    public PlayerGrindState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        
    }
    private SplineFollower sFollower;
    
    public override void Enter()
    {
        base.Enter();
        
        SetUpSplineFollower();
        
    }
    public override void Exit()
    {
        sFollower.enabled = false;
    }
    
    
    private void SetUpSplineFollower()
    {
        sFollower = player.GetComponent<SplineFollower>();
        sFollower.spline = player.GetCurrentSpline();
        sFollower.enabled = true;
        
        Vector3 playerForward = player.inputTurningTransform.forward;
        
        SplineSample sample = sFollower.spline.Project(player.transform.position);
        
        Vector3 splineTangent = sample.forward;

        
        if (player.rb.velocity.magnitude < 10) sFollower.followSpeed = 15;
        else sFollower.followSpeed = player.rb.velocity.magnitude - 10;

        // calculates the dot product of the player's velocity and the spline sample forward to determine if the player is moving forward or backward
        float dotProduct = Vector3.Dot(playerForward, splineTangent);

        // Set the SplineFollower to move forward or backward based on the dot product
        
        if (dotProduct > 0) // if the product is positive, that means we are moving in the direction of the spline
        {
            sFollower.followSpeed = Mathf.Abs(sFollower.followSpeed);
            sFollower.direction = Spline.Direction.Forward;
        }
        else
        {
            sFollower.followSpeed = -Mathf.Abs(sFollower.followSpeed);
            sFollower.direction = Spline.Direction.Backward;
        }

        sFollower.SetPercent(player.GetSplineCompletionPercent());
        sFollower.motion.offset = new Vector2(0, player.playerData.grindPositioningOffset);
    }

    private void JumpOffRail()
    {
        player.OllieJump();
        stateMachine.SwitchState(player.airborneState);
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        //player.GrindOnRail();
        //player.TurnPlayer();
    }
}
