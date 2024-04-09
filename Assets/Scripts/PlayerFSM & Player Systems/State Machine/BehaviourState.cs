using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BehaviourState
{
    protected PlayerBase player;
    protected PlayerStateMachine stateMachine;
    
    public struct InputActionEvents
    {
        public Action<InputAction.CallbackContext> onPerformed;
        public Action<InputAction.CallbackContext> onCanceled;
        // Add more actions as needed
    } // so we can subscribe different methods in states to multiple different input actions 
    
    protected Dictionary<InputAction, InputActionEvents> inputActions; // holds our input actions and their respective events
    
    protected BehaviourState(PlayerBase player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        inputActions = new Dictionary<InputAction, InputActionEvents>();
    }
    
    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        //UnsubscribeInputs();
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
            //Debug.Log("Subscribing to " + pair.Key);
            if (pair.Value.onPerformed != null) pair.Key.performed += pair.Value.onPerformed;
            if (pair.Value.onCanceled != null)  pair.Key.canceled += pair.Value.onCanceled;
        }
    }
    
    protected virtual void UnsubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            //Debug.Log("Unsubscribing from " + pair.Key);
            if (pair.Value.onPerformed != null) pair.Key.performed -= pair.Value.onPerformed;
            if (pair.Value.onCanceled != null) pair.Key.canceled -= pair.Value.onCanceled;
        }
    }
    
    
}
