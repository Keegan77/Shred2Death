using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ESF_EnterArena : EState_Flying
{
    public override void Enter ()
    {
        base.Enter ();
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


    /// <summary>
    /// After spawning and switcing to this state, follow pathpoint nodes of a given spawn point
    /// </summary>
    /// <returns></returns>
    IEnumerator EnterArena ()
    {
        Debug.Log ("Entering Arena");

        yield return MoveToObject(eFly.stateMachine.travelTarget.transform.GetChild(0).gameObject);

        for (int i = 0 ; i < eFly.stateMachine.travelTarget.transform.GetChild(1).childCount; i++)
        {
            yield return MoveToObject (eFly.stateMachine.travelTarget.transform.GetChild(1).GetChild (i).gameObject);
        }

        //yield return MoveToObject (eFly.stateMachine.travelTarget.transform.GetChild(2).gameObject);

        onPathComplete ();
    }

    public override void onPlayerSensorActivated ()
    {
        Debug.Log ("Aggro time");
        base.onPlayerSensorActivated ();
        eFly.stateMachine.transitionState (GetComponent<ES_Flying_Chase> ());
    }

    protected override void onPathComplete ()
    {
        base.onPathComplete ();
        e.stateMachine.transitionState (GetComponent<ES_Flying_Chase> ());
    }
}
