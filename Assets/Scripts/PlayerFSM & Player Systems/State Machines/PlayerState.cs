using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[Serializable]
public class PlayerState
{
    protected Dictionary<InputAction, InputActionEvents> behaviourInputActions;
    protected Dictionary<InputAction, InputActionEvents> abilityInputActions;

    public struct InputActionEvents
    {
        public Action<InputAction.CallbackContext> onPerformed;
        public Action<InputAction.CallbackContext> onCanceled;
        public Action<InputAction.CallbackContext> onStarted;
    }

    public PlayerState()
    {
        behaviourInputActions = new Dictionary<InputAction, InputActionEvents>();
        abilityInputActions = new Dictionary<InputAction, InputActionEvents>();
    }

    protected virtual void SubscribeInputs(bool abilityState = false)
    {
        if (!abilityState)
        {
            foreach (var pair in behaviourInputActions)
            {
                if (pair.Value.onPerformed != null) pair.Key.performed += pair.Value.onPerformed;
                if (pair.Value.onCanceled != null) pair.Key.canceled += pair.Value.onCanceled;
                if (pair.Value.onStarted != null) pair.Key.started += pair.Value.onStarted;
            }
        }
        else
        {
            foreach (var pair in abilityInputActions)
            {
                if (pair.Value.onPerformed != null) pair.Key.performed += pair.Value.onPerformed;
                if (pair.Value.onCanceled != null) pair.Key.canceled += pair.Value.onCanceled;
                if (pair.Value.onStarted != null) pair.Key.started += pair.Value.onStarted;
            }
        
        }

    }

    protected virtual void UnsubscribeInputs(bool abilityState = false)
    {
        if (!abilityState)
        {
            foreach (var pair in behaviourInputActions)
            {
                if (pair.Value.onPerformed != null) pair.Key.performed -= pair.Value.onPerformed;
                if (pair.Value.onCanceled != null) pair.Key.canceled -= pair.Value.onCanceled;
                if (pair.Value.onStarted != null) pair.Key.started -= pair.Value.onStarted;
            }
        }
        else
        {
            foreach (var pair in abilityInputActions)
            {
                if (pair.Value.onPerformed != null) pair.Key.performed -= pair.Value.onPerformed;
                if (pair.Value.onCanceled != null) pair.Key.canceled -= pair.Value.onCanceled;
                if (pair.Value.onStarted != null) pair.Key.started -= pair.Value.onStarted;
            }
        }
    }
}
