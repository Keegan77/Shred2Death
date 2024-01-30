using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Idle : Enemy_State
{
    [SerializeField] Enemy_State activateState;

    public override void Enter ()
    {
        Debug.Log ("Enemy does not detect the player and will be standing still");
    }
    public override void onPlayerSensorActivated ()
    {
        transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (activateState);
    }
}
