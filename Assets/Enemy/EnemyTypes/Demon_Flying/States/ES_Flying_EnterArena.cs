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
            e.stateMachine.travelPoint,
            Physics.Raycast(transform.position, e.stateMachine.travelPoint - transform.position, e.s_Spatial.maskRaycast) ? Color.red : Color.white
            );

        Debug.Break ();
    }

    public override void machineUpdate ()
    {
        base.machineUpdate ();

        Debug.DrawLine (
            transform.position,
            transform.position + (e.stateMachine.travelPoint - transform.position).normalized * e.s_Spatial.sensorLength,
            Physics.Raycast (transform.position, e.stateMachine.travelPoint - transform.position, e.s_Spatial.sensorLength, e.s_Spatial.maskRaycast) ? Color.red : Color.white
            );

        Debug.DrawLine (transform.position, transform.position + movementAvoidance, Color.green);

    }

    IEnumerator EnterArena ()
    {
        Debug.Log ("Entering Arena");

        for (int i = 0 ; i < e.stateMachine.travelTarget.transform.childCount; i++)
        {
            yield return MoveToObject (e.stateMachine.travelTarget.transform.GetChild (i).gameObject);
        }

        yield return MoveToObject (e.stateMachine.travelTarget);
    }
}
