using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerBase player;
    protected PlayerStateMachine stateMachine;

    protected PlayerState(PlayerBase player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    
    public virtual void Enter() { }

    public virtual void Exit() { }
    
    public virtual void LogicUpdate() { }
    
    public virtual void PhysicsUpdate() { }
    
    public virtual void HandleInput() { }

    public virtual void StateTriggerEnter(Collider other) { }
    
    public virtual void StateTriggerExit(Collider other) { }
    
    public virtual void StateTriggerStay(Collider other) { }
    
    
}
