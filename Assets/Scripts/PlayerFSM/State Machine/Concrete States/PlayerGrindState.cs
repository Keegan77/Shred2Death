using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    public PlayerGrindState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    private SplineFollower sFollower;
    
    public override void Enter()
    {
        InputRouting.Instance.input.Player.Jump.performed += ctx => JumpOffRail();
        base.Enter();
        player.StartCoroutine(SetUpSplineFollower());
    }
    public override void Exit()
    {
        InputRouting.Instance.input.Player.Jump.performed -= ctx => JumpOffRail();
    }

    private void JumpOffRail()
    {
        sFollower.enabled = false;
        player.SetRBKinematic(false);
        player.OllieJump();
        stateMachine.SwitchState(player.airborneState);
    }
    //coroutine for setting up the spline follower
    IEnumerator SetUpSplineFollower()
    {
        sFollower = player.sFollower;
        player.SetSplineFollowerActive(true);
        sFollower.motion.offset = new Vector2(0, player.grindPositionOffset);
        sFollower.spline = player.GetCurrentSpline();
        sFollower.followMode = SplineFollower.FollowMode.Uniform;
        sFollower.updateMethod = SplineFollower.UpdateMethod.Update;
        sFollower.followSpeed = 10;
        sFollower.wrapMode = SplineFollower.Wrap.Default;
        sFollower.autoStartPosition = false;
        sFollower.SetClipRange(player.GetSplineCompletionPercent(), 1);
        
        yield return new WaitUntil(() => sFollower.GetPercent() == 1);
        sFollower.SetClipRange(0, 1);
        sFollower.wrapMode = SplineFollower.Wrap.Loop;
        Debug.Log(sFollower.GetPercent());
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Debug.Log(sFollower.GetPercent());
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        //player.GrindOnRail();
        //player.TurnPlayer();
    }
}