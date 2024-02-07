using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Ground_MoveTowardsPoint : Enemy_State
{
    
    public override void Enter ()
    {
        e.agent.SetDestination (e.stateMachine.travelPoint);

        e.agent.CalculatePath (e.stateMachine.travelPoint, e.agentPath);
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
            e.stateMachine.transitionState (GetComponent<ES_Ground_Idle> ());
        }
    }

    public override void onPlayerSensorActivated ()
    {
        e.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        GetComponent<ES_Ground_Chase> ().onPlayerSensorActivated ();
    }
}
