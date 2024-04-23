using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Empty : Enemy_State
{
    public override void Enter ()
    {
        base.Enter ();
        e.animator.CrossFade (animationEnter, 0.5f);
    }
    public override void onPlayerSensorActivated ()
    {
        e.stateMachine.transitionState (GetComponent<ESG_Chase> ());
    }
}
