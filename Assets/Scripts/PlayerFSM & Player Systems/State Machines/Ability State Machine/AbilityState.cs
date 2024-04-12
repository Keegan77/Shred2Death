using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AbilityState : PlayerState
{
    protected PlayerBase player;
    protected AbilityStateMachine stateMachine;
    public AbilityStateMaps abilityStateMaps;
    
    protected AbilityState(PlayerBase player, AbilityStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        abilityStateMaps = new AbilityStateMaps(player);
    }
    
    /// <summary>
    /// Checks if the current behaviour state is banned from using the ability that is trying to be used. To see syntax
    /// for banning states, check the constructor of BoostAbilityState.cs
    /// </summary>
    /// <returns>True if the behaviour state is banned from the ability</returns>
    public bool StateIsBanned(BehaviourState state, AbilityState nextAbilityState)
    {
        if (!abilityStateMaps.abilityBannedStateMap.ContainsKey(nextAbilityState.GetType()))
        {
            return false;
        }
        return abilityStateMaps.abilityBannedStateMap[nextAbilityState.GetType()]
            .Contains(state.GetType());
    }

    
    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        if (StateIsBanned(player.stateMachine.currentState, stateMachine.currentAbilityState))
        {
            stateMachine.SwitchState(player.intermediaryAbilityState);
        }
    }
    
    public virtual void PhysicsUpdate() { }
    
}
