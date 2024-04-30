using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grounded enemies enter this state when they spawn. 
/// This state guides them towards a destination passed to them by a WaveManager.
/// </summary>
public class ESG_MoveTowardsPoint : ES_DemonGround
{
    public override void Enter ()
    {
        base.Enter ();

        //eGround.agent.SetDestination (eGround.stateMachine.travelPoint);
        //eGround.agent.CalculatePath (eGround.stateMachine.travelPoint, eGround.agentPath);
        StartCoroutine (MoveToPoint (eg.stateMachine.travelTarget.transform.GetChild(2).position, animationEnter));

    }


    /// <summary>
    /// When the enemy gets close enough to their destination, begin chasing the player.
    /// </summary>
    public override void machinePhysics ()
    {
        Vector3 distanceToDestination = eg.stateMachine.travelPoint - transform.position;

        if (distanceToDestination.magnitude <= eg.agent.stoppingDistance)
        {
            Debug.Log ("Transition requirement met");
            Debug.Log (eg.agent.destination);
            Debug.Log (transform.position);
            Debug.Log(distanceToDestination.magnitude);
            eg.stateMachine.transitionState(GetComponent<ESG_Chase>());
        }
    }

    public override void onPlayerSensorActivated ()
    {
        eg.stateMachine.transitionState (GetComponent<ESG_Chase> ());
        GetComponent<ESG_Chase> ().onPlayerSensorActivated ();
    }

    protected override void OnDestinationReached ()
    {
        eg.stateMachine.transitionState(GetComponent<ESG_Chase> ());
    }

    protected override void OnDestinationFailed ()
    {
        Debug.LogWarning ($"{name} could not navigate path to entry point");
    }
}
