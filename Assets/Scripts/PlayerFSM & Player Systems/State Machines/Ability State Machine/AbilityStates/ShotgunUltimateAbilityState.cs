using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunUltimateAbilityState : AbilityState
{
    public ShotgunUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        
    }
    
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Shotgun Ultimate Entered");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
