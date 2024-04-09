using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediaryAbilityState : AbilityState
{
    public IntermediaryAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        abilityInputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents()
        {
            onPerformed = ctx => stateMachine.SwitchState(player.boostAbilityState)
        });
    }

    public override void Enter()
    {
        base.Enter();
        SubscribeInputs(abilityState:true);
        Debug.Log("Intermediary Entered");
    }

    public override void Exit()
    {
        UnsubscribeInputs(abilityState:true);
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
