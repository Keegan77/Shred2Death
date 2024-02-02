using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ES_Chase : Enemy_State
{
    [SerializeField] float agentUpdateDistance = 4;

    public override void Enter ()
    {
        e.agent.SetDestination (Enemy.playerObject.transform.position);
    }
    public override void onPlayerSensorDeactivated ()
    {
        //transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (returnState);
        constantUpdate = false;
    }

    public override void onPlayerSensorActivated ()
    {
        constantUpdate = true;
    }

    //If the player goes too far from the destination point,
    //calculate a new destination towards the player

    bool constantUpdate = false;
    public override void machinePhysics ()
    {
        Vector3 playerDestinationOffset = Enemy.playerObject.transform.position - e.agent.destination;


        if (constantUpdate)
        {
            e.agent.SetDestination (Enemy.playerObject.transform.position);

            Debug.Log (e.agent.pathStatus);
        }
        else if (playerDestinationOffset.magnitude > agentUpdateDistance)
        {
            e.agent.SetDestination(Enemy.playerObject.transform.position);
            Debug.Log ("Resetting Path");
        }

        
    }
}
