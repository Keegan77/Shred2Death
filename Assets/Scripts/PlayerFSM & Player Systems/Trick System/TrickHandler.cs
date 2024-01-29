using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PrintCurrentTrickNames();
    }

    private void UpdateTrickList()
    {
        if (TrickMaps.StateMap.ContainsKey(player.stateMachine.currentState.GetType()))
        {
            DeleteTrickInputs();
            currentTricks = TrickMaps.StateMap[player.stateMachine.currentState.GetType()];
            SetUpTrickInputs();
            
            PrintCurrentTrickNames();
        }
        else
        {
            DeleteTrickInputs();
            Debug.LogError("No tricks found for this state!");
        }
    }

    private void SetUpTrickInputs()
    {
        foreach (var trick in currentTricks)
        {
            //trick.trickAction.performed += ctx => player.anim.SetTrigger(trick.animTriggerName);
            trick.trickAction.performed += ctx => DoTrick(trick);
        }
    }

    private void DeleteTrickInputs()
    {
        foreach (var trick in currentTricks)
        {
            //trick.trickAction.performed -= ctx => player.anim.SetTrigger(trick.animTriggerName);
            trick.trickAction.performed -= ctx => DoTrick(trick);
        }
    }

    private void DoTrick(Trick trick)
    {
        throw new NotImplementedException();
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
    }
}
