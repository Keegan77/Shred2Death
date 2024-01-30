using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//When not chasing the player,
//pick random points to navigate to every so often.
//The idea is to appear to wander aimlessly,
//but in reality movement is biased towards the player.
public class ES_Idle : Enemy_State
{
    [SerializeField] float wanderSearchRadius = 10;
    [SerializeField] float wanderPlayerBias = 5; //offset the search area towards the player
    [SerializeField] float wanderSearchIterations = 25;
    [SerializeField] float wanderStayTime;
    public override void Enter ()
    {
        //if(SetWanderPoint (out wanderResult))
        //{
        //    e.agent.SetDestination (wanderResult);
        //}
        //if (e.agent.CalculatePath (playerObject.transform.position, e.agentPath))
        //{
        //    Debug.Log ("Pathfinding via Calculatepath successful");
        //}

        if (SetWanderPoint())
        {
            e.agent.SetDestination(wanderResult);
        }

    }

    public override void Exit ()
    {
        e.agent.ResetPath ();
    }

    //If enemy reaches their wandering destination and isn't already waiting,
    //Start the WanderTimer
    public override void machinePhysics ()
    {
        Vector3 destinationOffset = e.agent.destination - transform.position;

        if (destinationOffset.magnitude < e.agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine (WanderTimer());
        }

    }

    public override void onPlayerSensorActivated ()
    {
        transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (GetComponent<ES_Chase>());
    }

    //calculates the path an enemy would take to reach the player,
    //and then uses the first point along that path as a direction for the wander bias.

    Vector3 wanderResult = Vector3.zero;
    public bool SetWanderPoint ()
    {
        Vector3 wanderOffset = Vector3.zero;

        //Calculate a path to the player
        e.agent.CalculatePath (playerObject.transform.position, e.agentPath);

        Debug.Log(e.agentPath.status);

        //If the path is incomplete return a null path
        if (e.agentPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log ($"{gameObject.name}: Wander could not find path to player");

            wanderResult = Vector3.zero;
            return false;
        }

        Debug.Log ($"Wander Path Size: {e.agentPath.corners.Length}");

        foreach (Vector3 i in e.agentPath.corners)
        {
            Debug.Log ("Enemypath point " + i);
        }

        //Get the offset of the path with a restriciton of the wander bias
        if (e.agentPath.corners.Length > 0)
        {
            wanderOffset = e.agentPath.corners[1] - transform.position;

            //If the corner is further than the bias shorten it down
            if (wanderOffset.magnitude > wanderPlayerBias)
            {
                wanderOffset = wanderOffset.normalized * wanderPlayerBias;
            }
        }

        //if the path array is empty don't calculate wander offset using the path.
        //Likely not an extant scenario because we're only operating on completed paths here.
        else
        {
            Debug.LogError ($"{gameObject.name} Path Array is empty");
        }


        Vector3 point = Vector3.zero;
        //We have the wander offset now. Search this area for the first candidate point in that area.
        for (int i = 0; i < wanderSearchIterations; i++)
        {
            //point = wanderOffset + UnityEngine.Random.insideUnitSphere.normalized * wanderSearchRadius;
            Debug.Log ("Sampling point" + point);
            point = e.agentPath.corners[1];

            NavMeshHit hit;

            //If a point on the navmesh was found
            if (NavMesh.SamplePosition(point, out hit, 1, NavMesh.AllAreas))
            {
                Debug.Log ("Navmesh wander point found");

                wanderResult = hit.position;
                return true;
            }
        }

        //if none of the points in the navmesh search were valid:

        Debug.Log ("After searching, wander did not find any points");
        wanderResult = Vector3.zero;
        return false;
    }


    bool isWaiting = false;
    IEnumerator WanderTimer ()
    {
        isWaiting = true;
        yield return new WaitForSeconds (wanderStayTime);

        if (SetWanderPoint())
        {
            e.agent.SetDestination (wanderResult);
        }

        isWaiting = false;
    }
}
