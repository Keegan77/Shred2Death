using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player tricks off of an enemy, they stop what they're doing and play a bonk animation.
/// Once the animation is finished, 
/// </summary>
public class ES_Bonk : Enemy_State
{
    public Enemy_State stateNext;
    public override void Enter ()
    {
        base.Enter ();
        e.animator.Play (animationEnter);
    }

    public override void OnAnimationFinished ()
    {
        e.stateMachine.transitionState (stateNext);
    }
}
