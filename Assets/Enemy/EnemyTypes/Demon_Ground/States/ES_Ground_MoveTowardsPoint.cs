using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Ground_MoveTowardsPoint : ES_DemonGround
{
    
    public override void Enter ()
    {
        eGround.agent.SetDestination (eGround.stateMachine.travelPoint);

        eGround.agent.CalculatePath (eGround.stateMachine.travelPoint, eGround.agentPath);
    }

    public override void Exit ()
    {
        eGround.agent.ResetPath ();
    }

    public override void machinePhysics ()
    {
        Vector3 distanceToDestination = eGround.agent.destination - transform.position;

        if (distanceToDestination.magnitude <= eGround.agent.stoppingDistance)
        {
            eGround.stateMachine.transitionState (GetComponent<ES_Ground_Idle> ());
        }
    }

    public override void onPlayerSensorActivated ()
    {
        eGround.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        GetComponent<ES_Ground_Chase> ().onPlayerSensorActivated ();
    }
}
