using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//When not chasing the player,
//pick random points to navigate to every so often.
//The idea is to appear to wander aimlessly,
//but in reality movement is biased towards the player.
public class ES_Idle : Enemy_State
{
    [SerializeField] float wanderSearchRadius = 10;
    [SerializeField] float wanderPlayerBias = 5; //offset the search area towards the player
    [SerializeField] float wanderSearchIterations = 25;
    [SerializeField] float wanderStayTimeMin;
    [SerializeField] float wanderStayTimeMax;


    //Using this to find where unity is searching
    Vector3 debugPoint;
    Vector3 debugPointCorner;
    Vector3 debugPointPlayer;
    Vector3 debugPointSearch;
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

    public override void machineUpdate ()
    {
        base.machineUpdate ();
        Debug.DrawLine (debugPoint, debugPoint + new Vector3(0, 5, 0), Color.red);
        Debug.DrawLine (debugPointCorner, debugPointCorner + new Vector3 (0, 5, 0), Color.green);
        Debug.DrawLine (debugPointPlayer, debugPointPlayer + new Vector3 (0, 5, 0), Color.blue);

        Debug.DrawLine (groundHit.point, groundHit.point + new Vector3 (0, 10, 0), Color.white);
        Debug.DrawLine (debugPointSearch, debugPointSearch + new Vector3 (0, 5, 0), Color.yellow);
    }

    public override void onPlayerSensorActivated ()
    {
        e.stateMachine.transitionState (GetComponent<ES_Chase>());
        GetComponent<ES_Chase>().onPlayerSensorActivated ();
    }

    //calculates the path an enemy would take to reach the player,
    //and then uses the first point along that path as a direction for the wander bias.

    Vector3 wanderResult = Vector3.zero;
    RaycastHit groundHit;
    Vector3 pointSearchOrigin;
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
            Debug.DrawLine (i, i + new Vector3 (0, 1, 0), Color.red);
        }

        //Get the offset of the path with a restriciton of the wander bias
        if (e.agentPath.corners.Length > 0)
        {

            wanderOffset = transform.position + (e.agentPath.corners[1] - transform.position);

            Debug.Log (wanderOffset);
            Debug.Log (wanderOffset.magnitude);
            //If the corner is further than the bias shorten it down
            if (wanderOffset.magnitude > wanderPlayerBias)
            {
                Debug.Log ("Adjusting wanderoffset");
                wanderOffset = transform.position + (wanderOffset - transform.position).normalized * wanderPlayerBias;
            }
        }

        //if the path array is empty don't calculate wander offset using the path.
        //Likely not an extant scenario because we're only operating on completed paths here.
        else
        {
            Debug.LogError ($"{gameObject.name} Path Array is empty");
        }


        //The search are is defined by getting a point in a 2D circle around the player,
        //and then firing a raycast downwards towards the ground as a way to search for valid points
        

        //If you hit the ceiling how much are you subtracting from the height of the downwards raycast
        //

        RaycastHit ceilingCheck;
        Vector3 ceilingPoint = Vector3.zero;
        //float searchDistanceJump = 0;
        float searchDistanceDrop = Enemy.agentSettings[e.agentIndex].ledgeDropHeight;
        
        if (Physics.Raycast (transform.position, Vector3.up, out ceilingCheck, Enemy.agentSettings[e.agentIndex].maxJumpAcrossDistance, LayerMask.GetMask ("Ground")))
        {
            Debug.Log ("Raycast hit something");
            //Debug.Log (ceilingCheck.point);
            //searchDistanceJump = ceilingCheck.point.y;

            ceilingPoint = ceilingCheck.point;
        }
        else
        {
            Debug.Log ("Raycast did not hit something");
            //Debug.Log (ceilingCheck.point);
            //searchDistanceJump = transform.position.y + Enemy.agentSettings[e.agentIndex].maxJumpAcrossDistance;

            ceilingPoint = transform.position + new Vector3 (0, Enemy.agentSettings[e.agentIndex].maxJumpAcrossDistance, 0);
        }

        

        Vector3 point = Vector3.zero;
        //We have the wander offset now. Search this area for the first candidate point in that area.
        for (int i = 0; i < wanderSearchIterations; i++)
        {
            //point = wanderOffset + UnityEngine.Random.insideUnitSphere.normalized * wanderSearchRadius;

            Vector2 pointFlat = UnityEngine.Random.insideUnitCircle.normalized * wanderSearchRadius;
            //point = wanderOffset + new Vector3 (pointFlat.x, transform.position.y, pointFlat.y);
            point = wanderOffset + new Vector3 (pointFlat.x, ceilingPoint.y, pointFlat.y);

            //pointSearchOrigin = new Vector3 (point.x, ceilingPoint.y, point.y);

            Debug.Log ((point.y - transform.position.y) + searchDistanceDrop);
            //From the point, 
            if (Physics.Raycast (point, Vector3.down, out groundHit, (point.y - transform.position.y) + searchDistanceDrop, LayerMask.GetMask ("Ground")))
            {
                Debug.Log ("Raycastpoint found");
                Debug.Log (groundHit.point);
                Debug.DrawLine (point, new Vector3 (point.x, transform.position.y - searchDistanceDrop, point.z), Color.black);
                
            }
            else
            {
                Debug.Log ("Raycastpoint not found");
                
            }
            

            Debug.Log ("Sampling point" + point);


            debugPoint = e.agentPath.corners[1];
            debugPointCorner = wanderOffset;
            debugPointPlayer = transform.position;

            
            //Debug.DrawLine (pointSearchOrigin, new Vector3 (pointSearchOrigin.x, 0, pointSearchOrigin.z), Color.white);
            NavMeshHit navHit;

            //If a point on the navmesh was found
            if (NavMesh.SamplePosition(groundHit.point, out navHit, 1, NavMesh.AllAreas))
            {
                Debug.Log ("Navmesh wander point found");

                wanderResult = navHit.position;
                debugPointSearch = navHit.position;


                Debug.Break ();
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
        yield return new WaitForSeconds (Random.Range(wanderStayTimeMin, wanderStayTimeMax));

        if (SetWanderPoint())
        {
            e.agent.SetDestination (wanderResult);
        }

        isWaiting = false;
    }
}
