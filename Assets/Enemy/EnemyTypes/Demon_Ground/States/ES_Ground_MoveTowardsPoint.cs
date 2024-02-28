using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Ground_MoveTowardsPoint : ES_DemonGround
{
    public override void Enter ()
    {
        base.Enter ();

        //eGround.agent.SetDestination (eGround.stateMachine.travelPoint);
        //eGround.agent.CalculatePath (eGround.stateMachine.travelPoint, eGround.agentPath);
        StartCoroutine (MoveToPoint (eg.stateMachine.travelPoint, animationEnter));

    }

    public override void machinePhysics ()
    {
        //Vector3 distanceToDestination = eGround.agent.destination - transform.position;

        //if (distanceToDestination.magnitude <= eGround.agent.stoppingDistance)
        //{
        //    eGround.stateMachine.transitionState (GetComponent<ES_Ground_Idle> ());
        //}
    }

    public override void onPlayerSensorActivated ()
    {
        eg.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        GetComponent<ES_Ground_Chase> ().onPlayerSensorActivated ();
    }

    protected override void OnDestinationReached ()
    {
        eg.stateMachine.transitionState(GetComponent<ES_Ground_Idle> ());
    }

    protected override void OnDestinationFailed ()
    {
        Debug.LogWarning ($"{name} could not navigate path to entry point");
    }
}
