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
        base.Enter();
        player.SetRBKinematic(true);
        sFollower = player.AddComponent<SplineFollower>();
        sFollower.spline = player.currentSpline;
        sFollower.followMode = SplineFollower.FollowMode.Uniform;
        sFollower.updateMethod = SplineFollower.UpdateMethod.Update;
        sFollower.followSpeed = 10;
        sFollower.wrapMode = SplineFollower.Wrap.Loop;
        sFollower.SetPercent(player.splineCompletionPercent);
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
