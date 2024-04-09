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
    
    public virtual void Enter() { }

    public virtual void Exit() { }
    
    public virtual void LogicUpdate() { }
    
    public virtual void PhysicsUpdate() { }
    
}
