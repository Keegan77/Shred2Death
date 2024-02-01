using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerBase player;
    protected PlayerStateMachine stateMachine;
    
    protected Dictionary<InputAction, Action<InputAction.CallbackContext>> inputActions;

    protected PlayerState(PlayerBase player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        inputActions = new Dictionary<InputAction, Action<InputAction.CallbackContext>>();
    }

    public virtual void Enter()
    {
        SubscribeInputs();
    }

    public virtual void Exit()
    {
        UnsubscribeInputs();
    }
    
    public virtual void LogicUpdate() { }
    
    public virtual void PhysicsUpdate() { }

    public virtual void StateTriggerEnter(Collider other) { }
    
    public virtual void StateTriggerExit(Collider other) { }
    
    public virtual void StateTriggerStay(Collider other) { }
    
    public virtual void StateCollisionEnter(Collision other) { }

    protected virtual void SubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            pair.Key.performed += pair.Value;
        }
    }
    
    protected virtual void UnsubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            pair.Key.performed -= pair.Value;
        }
    }
    
    
}
