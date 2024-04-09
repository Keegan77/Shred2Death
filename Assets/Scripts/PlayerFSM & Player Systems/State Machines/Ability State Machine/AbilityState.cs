using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState : PlayerState
{
    protected PlayerBase player;
    protected AbilityStateMachine stateMachine;
    
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
