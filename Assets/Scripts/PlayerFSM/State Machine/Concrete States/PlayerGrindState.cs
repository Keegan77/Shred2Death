using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
        player.StartCoroutine(SetUpSplineFollower());
    }
    public override void Exit()
    {
    }


    //coroutine for setting up the spline follower
    IEnumerator SetUpSplineFollower()
    {
        sFollower = player.AddComponent<SplineFollower>();
        sFollower.motion.offset = new Vector2(0, player.grindPositionOffset);
        sFollower.spline = player.GetCurrentSpline();
        sFollower.followMode = SplineFollower.FollowMode.Uniform;
        sFollower.updateMethod = SplineFollower.UpdateMethod.Update;
        sFollower.followSpeed = player.GetCurrentSpeed();
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
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        //player.GrindOnRail();
        //player.TurnPlayer();
    }
}
