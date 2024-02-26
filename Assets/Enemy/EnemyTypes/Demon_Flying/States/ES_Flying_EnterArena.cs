using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ES_Flying_EnterArena : EState_Flying
{
    public override void Enter ()
    {
        //StartCoroutine (MoveToObject (e.stateMachine.travelTarget.transform.parent.gameObject));
        StartCoroutine(EnterArena ());
    }

    public void testAvoidancePath ()
    {
        Debug.DrawLine(
            transform.position, 
            eFly.stateMachine.travelPoint,
            Physics.Raycast(transform.position, eFly.stateMachine.travelPoint - transform.position, eFly.s_Spatial.maskRaycast) ? Color.red : Color.white
            );

        Debug.Break ();
    }

    public override void machineUpdate ()
    {
        base.machineUpdate ();
    }

    IEnumerator EnterArena ()
    {
        Debug.Log ("Entering Arena");

        for (int i = 0 ; i < eFly.stateMachine.travelTarget.transform.parent.GetChild(1).childCount; i++)
        {
            yield return MoveToObject (eFly.stateMachine.travelTarget.transform.parent.GetChild(1).GetChild (i).gameObject);
        }

        yield return MoveToObject (eFly.stateMachine.travelTarget);
    }

    public override void onPlayerSensorActivated ()
    {
        base.onPlayerSensorActivated ();
        eFly.stateMachine.transitionState (GetComponent<ES_Flying_Chase> ());
    }
}
