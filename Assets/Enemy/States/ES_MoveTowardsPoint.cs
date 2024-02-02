using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ES_MoveTowardsPoint : Enemy_State
{
    [NonSerialized] public GameObject travelPoint;

    public override void Enter ()
    {
        e.agent.SetDestination (travelPoint.transform.position);

        e.agent.CalculatePath (travelPoint.transform.position, e.agentPath);

    }

    public override void Exit ()
    {
        e.agent.ResetPath ();
    }

    public override void machinePhysics ()
    {
        Vector3 distanceToDestination = e.agent.destination - transform.position;

        if (distanceToDestination.magnitude <= e.agent.stoppingDistance)
        {
            e.stateMachine.transitionState (GetComponent<ES_Idle> ());
        }
    }

    public override void onPlayerSensorActivated ()
    {
        e.stateMachine.transitionState (GetComponent<ES_Chase> ());
        GetComponent<ES_Chase> ().onPlayerSensorActivated ();
    }

}
