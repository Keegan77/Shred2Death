using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStateMachine
{
    public AbilityState currentAbilityState;
    public void Init(AbilityState startingState)
    {
        currentAbilityState = startingState;
        currentAbilityState.Enter();
    }
    
    public void SwitchState(AbilityState newState)
    {
        currentAbilityState.Exit();
        currentAbilityState = newState;
        currentAbilityState.Enter();
    }
}
