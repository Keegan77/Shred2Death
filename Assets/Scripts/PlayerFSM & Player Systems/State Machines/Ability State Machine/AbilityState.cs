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
    public bool CurrentStateIsBanned()
    {
        if (!abilityStateMaps.abilityBannedStateMap.ContainsKey(player.abilityStateMachine.currentAbilityState.GetType()))
        {
            return false;
        }
        
        return abilityStateMaps.abilityBannedStateMap[player.abilityStateMachine.currentAbilityState.GetType()]
            .Contains(player.stateMachine.currentState);
    }
    
    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        if (CurrentStateIsBanned())
        {
            stateMachine.SwitchState(player.intermediaryAbilityState);
        }
    }
    
    public virtual void PhysicsUpdate() { }
    
}
