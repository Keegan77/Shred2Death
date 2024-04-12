using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualieUltimateAbilityState : AbilityState
{
    public DualieUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Dualie Ultimate Entered");
    }
    
    
}

