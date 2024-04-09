using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediaryAbilityState : AbilityState
{
    public IntermediaryAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
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
