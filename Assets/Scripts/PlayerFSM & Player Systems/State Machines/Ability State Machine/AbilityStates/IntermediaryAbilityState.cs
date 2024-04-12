using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediaryAbilityState : AbilityState
{
    public IntermediaryAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        abilityInputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents()
        {
            onPerformed = ctx =>
            {
                RequestNewState(player.boostAbilityState);
            },
        });
        abilityInputActions.Add(InputRouting.Instance.input.Player.Ability, new InputActionEvents()
        {
            onPerformed = ctx =>
            {
                switch (player.gunfireHandler.GetCurrentGunData().name)
                {
                    case "Dualies":
                        RequestNewState(player.dualieUltimateAbilityState);
                        break;
                    case "Shotgun":
                        RequestNewState(player.shotgunUltimateAbilityState);
                        break;
                }
            },
        });
        
    }

    private void RequestNewState(AbilityState state)
    {
        if (StateIsBanned(player.stateMachine.currentState, state)) return; //if current state banned from next ab state, leave
        if (player.GetComboHandler().GetStylePoints() < abilityStateMaps.abilityStyleCostMap[state.GetType()]) return;
        // we meet the pre-requisites to enter the next ability
        player.GetComboHandler().DecrementStylePoints(abilityStateMaps.abilityStyleCostMap[state.GetType()]);
        stateMachine.SwitchState(state);
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
