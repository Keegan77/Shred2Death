using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState;
    
    public void Init(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
    
    public void SwitchState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}