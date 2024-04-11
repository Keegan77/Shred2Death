using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AbilityState : PlayerState
{
    protected PlayerBase player;
    protected AbilityStateMachine stateMachine;

    //dictionary to link ability states to the behaviour states that are banned for that state
    public Dictionary<Type, List<BehaviourState>> abilityBannedStateMap = 
        new Dictionary<Type, List<BehaviourState>>();
    
    protected AbilityState(PlayerBase player, AbilityStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    
    /// <summary>
    /// Checks if the current behaviour state is banned from using the ability that is trying to be used. To see syntax
    /// for banning states, check the constructor of BoostAbilityState.cs
    /// </summary>
    /// <returns>True if the behaviour state is banned from the ability</returns>
    public bool CurrentStateIsBanned()
    {
        if (!abilityBannedStateMap.ContainsKey(player.abilityStateMachine.currentAbilityState.GetType()))
        {
            return false;
        }
        
        return abilityBannedStateMap[player.abilityStateMachine.currentAbilityState.GetType()]
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
