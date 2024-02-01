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
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents { onPerformed = ctx => JumpOffRail()});
    }
    private SplineFollower sFollower;
    
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs();
        SetUpSplineFollower();
        
    }
    public override void Exit()
    {
        UnsubscribeInputs();
        sFollower.enabled = false;
        sFollower.spline = null;
    }
    
    
    private void SetUpSplineFollower()
    {
        sFollower = player.GetComponent<SplineFollower>();
        sFollower.spline = player.GetCurrentSpline();
        sFollower.enabled = true;
        
        Vector3 playerForward = player.inputTurningTransform.forward;
        
        SplineSample sample = sFollower.spline.Project(player.transform.position);
        
        Vector3 splineTangent = sample.forward;

        
        if (player.rb.velocity.magnitude < 15) sFollower.followSpeed = 15;
        else sFollower.followSpeed = player.rb.velocity.magnitude;

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
        player.rb.AddForce(player.transform.up * player.playerData.baseJumpForce, ForceMode.Impulse);
        player.rb.AddForce(player.inputTurningTransform.forward * player.playerData.baseJumpForce, ForceMode.Impulse);
        //player.OllieJump();
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
