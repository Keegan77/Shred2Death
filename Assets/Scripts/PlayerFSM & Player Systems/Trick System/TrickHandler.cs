using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBase))]
public class TrickHandler : MonoBehaviour
{
    private PlayerBase player;

    private Trick[] currentTricks;
    private void Awake()
    {
        player = GetComponent<PlayerBase>();
    }

    private void Start()
    {
        currentTricks = TrickMaps.StateMap[player.stateMachine.currentState.GetType()];
        SetUpTrickInputs();
        //PrintCurrentTrickNames();
    }

    private void UpdateTrickList()
    {
        DeleteTrickInputs(); // Unsubscribes from old trick inputs before setting new ones
        if (TrickMaps.StateMap.ContainsKey(player.stateMachine.currentState.GetType()))
        {
            currentTricks = TrickMaps.StateMap[player.stateMachine.currentState.GetType()];
            SetUpTrickInputs(); // Subscribes to new trick inputs
            
            //PrintCurrentTrickNames();
        }
        else
        {
            DeleteTrickInputs();
            Debug.LogError("No tricks found for this state!");
        }
    }

    private Dictionary<Trick, Action<InputAction.CallbackContext>> trickActions = new Dictionary<Trick, Action<InputAction.CallbackContext>>();

    private void SetUpTrickInputs()
    {
        foreach (var trick in currentTricks)
        {
            Action<InputAction.CallbackContext> action = ctx => DoTrick(trick);
            trick.trickAction.performed += action;
            trickActions[trick] = action;
        }
    }

    private void DeleteTrickInputs()
    {
        foreach (var trick in currentTricks)
        {
            if (trickActions.TryGetValue(trick, out var action))
            {
                trick.trickAction.performed -= action;
                trickActions.Remove(trick);
            }
        }
    }

    private void DoTrick(Trick trick)
    {
        ActionEvents.OnTrickPerformed?.Invoke(trick);
        Debug.Log($"Trick Performed: {trick.animTriggerName}");
    }

    private void PrintCurrentTrickNames()
    {
        foreach (var trick in currentTricks)
        {
            Debug.Log(trick.animTriggerName);
        }
    }
    
    private void OnEnable()
    {
        ActionEvents.OnPlayerStateSwitch += UpdateTrickList;
    }

    private void OnDisable()
    {
        ActionEvents.OnPlayerStateSwitch -= UpdateTrickList;
        DeleteTrickInputs();
    }
}
