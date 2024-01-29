using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Chase : Enemy_State
{
    [SerializeField] Enemy_State returnState;

    public override void Enter ()
    {
        base.Enter ();
        Debug.Log ("Enemy has detected player and will begin chasing them");
    }
    public override void onPlayerSensorDeactivated ()
    {
        transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (returnState);
    }
}
