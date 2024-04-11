using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public BehaviourState currentState;
    public void Init(BehaviourState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
    
    public void SwitchState(BehaviourState newState)
    {
        currentState.Exit();
        currentState = newState;
        ActionEvents.OnBehaviourStateSwitch?.Invoke(currentState.GetType()); // invokes only on behaviour states & not ability states
        currentState.Enter();
    }
}
