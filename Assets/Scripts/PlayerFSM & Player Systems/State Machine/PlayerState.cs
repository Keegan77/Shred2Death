using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerBase player;
    protected PlayerStateMachine stateMachine;
    
    public struct InputActionEvents
    {
        public Action<InputAction.CallbackContext> onPerformed;
        public Action<InputAction.CallbackContext> onCanceled;
        // Add more actions as needed
    } // so we can subscribe different contexts to methods from states
    
    protected Dictionary<InputAction, InputActionEvents> inputActions;
    


    protected PlayerState(PlayerBase player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        inputActions = new Dictionary<InputAction, InputActionEvents>();
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
            pair.Key.performed += pair.Value.onPerformed;
            pair.Key.canceled += pair.Value.onCanceled;
        }
    }
    
    protected virtual void UnsubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            pair.Key.performed -= pair.Value.onPerformed;
            pair.Key.canceled -= pair.Value.onCanceled;
        }
    }
    
    
}
