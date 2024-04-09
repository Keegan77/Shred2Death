using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerState
{
    protected Dictionary<InputAction, InputActionEvents> inputActions;

    public struct InputActionEvents
    {
        public Action<InputAction.CallbackContext> onPerformed;
        public Action<InputAction.CallbackContext> onCanceled;
    }

    public PlayerState()
    {
        inputActions = new Dictionary<InputAction, InputActionEvents>();
    }

    protected virtual void SubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            if (pair.Value.onPerformed != null) pair.Key.performed += pair.Value.onPerformed;
            if (pair.Value.onCanceled != null) pair.Key.canceled += pair.Value.onCanceled;
        }
    }

    protected virtual void UnsubscribeInputs()
    {
        foreach (var pair in inputActions)
        {
            if (pair.Value.onPerformed != null) pair.Key.performed -= pair.Value.onPerformed;
            if (pair.Value.onCanceled != null) pair.Key.canceled -= pair.Value.onCanceled;
        }
    }
}
