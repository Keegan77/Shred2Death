using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Empty : Enemy_State
{
    public override void Enter ()
    {
        base.Enter ();
        e.animator.Play (animationEnter);
    }
    public override void onPlayerSensorActivated ()
    {
        e.stateMachine.transitionState (GetComponent<ESG_Chase> ());
    }
}
