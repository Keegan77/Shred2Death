using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    public PlayerGrindState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    
    
    public override void Enter()
    {
        base.Enter();
        player.SetRBKinematic(true);
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.GrindOnRail();
        //player.TurnPlayer();
    }
}
