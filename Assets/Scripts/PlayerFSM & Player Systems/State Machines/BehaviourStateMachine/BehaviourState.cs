using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BehaviourState : PlayerState
{
    protected PlayerBase player;
    protected PlayerStateMachine stateMachine;
    
    protected BehaviourState(PlayerBase player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    
    public virtual void Enter() { }

    public virtual void Exit() { }
    
    public virtual void LogicUpdate() { }
    
    public virtual void PhysicsUpdate() { }

    public virtual void StateTriggerEnter(Collider other) { }
    
    public virtual void StateTriggerExit(Collider other) { }
    
    public virtual void StateTriggerStay(Collider other) { }
    
    public virtual void StateCollisionEnter(Collision other) { }
    
}
