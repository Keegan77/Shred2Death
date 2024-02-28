using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ES_Ground_Idle : ES_DemonGround
{
    [SerializeField] string animationWalk = "";

    [Header("Wander Paramaters")]
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
    Vector3 debugPointGround;

    #region STATE MACHINE
    public override void Enter ()
    {
        base.Enter ();
        if (SetWanderPoint ())
        {
            StartCoroutine (MoveToPoint(wanderResult, animationWalk));
        }
        else
        {
            StartCoroutine (WanderTimer ());
        }
    }

    //If enemy reaches their wandering destination and isn't already waiting,
    //Start the WanderTimer
    public override void machinePhysics ()
    {
    }

    public override void machineUpdate ()
    {
        base.machineUpdate ();
        Debug.DrawLine (debugPoint, debugPoint + new Vector3 (0, 5, 0), Color.red);
        Debug.DrawLine (debugPointCorner, debugPointCorner + new Vector3 (0, 5, 0), Color.green);
        Debug.DrawLine (debugPointPlayer, debugPointPlayer + new Vector3 (0, 5, 0), Color.blue);

        Debug.DrawLine (debugPointGround, debugPointGround + new Vector3 (0, 10, 0), Color.white);
        Debug.DrawLine (debugPointSearch, debugPointSearch + new Vector3 (0, 5, 0), Color.yellow);
    }

    public override void onPlayerSensorActivated ()
    {
        eg.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        GetComponent<ES_Ground_Chase> ().onPlayerSensorActivated ();
    }

    #endregion

    //calculates the path an enemy would take to reach the player,
    //and then uses the first point along that path as a direction for the wander bias.

    Vector3 wanderResult = Vector3.zero;
    public bool SetWanderPoint ()
    {
        Vector3 wanderOffset = Vector3.zero;

        //Calculate a path to the player
        eg.agent.CalculatePath (Enemy.playerReference.transform.position, eg.agentPath);
        Debug.Log (Enemy.playerReference.transform.position);

        Debug.Log (eg.agentPath.status);

        //If the path is incomplete return a null path
        if (eg.agentPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log ($"{gameObject.name}: Wander could not find path to player");

            wanderResult = Vector3.zero;
            return false;
        }

        Debug.Log ($"Wander Path Size: {eg.agentPath.corners.Length}");

        foreach (Vector3 i in eg.agentPath.corners)
        {
            Debug.Log ("Enemypath point " + i);
            Debug.DrawLine (i, i + new Vector3 (0, 1, 0), Color.red);
        }

        //Get the offset of the path with a restriciton of the wander bias
        if (eg.agentPath.corners.Length > 0)
        {

            wanderOffset = transform.position + (eg.agentPath.corners[1] - transform.position);

            //Debug.Log (wanderOffset);
            //Debug.Log (wanderOffset.magnitude);
            //If the corner is further than the bias shorten it down
            if (wanderOffset.magnitude > wanderPlayerBias)
            {
                //Debug.Log ("Adjusting wanderoffset");
                wanderOffset = transform.position + (wanderOffset - transform.position).normalized * wanderPlayerBias;
            }
        }

        //if the path array is empty don't calculate wander offset using the path.
        //Likely not an extant scenario because we're only operating on completed paths here.
        else
        {
            Debug.LogError ($"{gameObject.name} Path Array is empty");
        }


        //The search area is defined by getting a point in a 2D circle around the player,
        //and then firing a raycast downwards towards the ground as a way to search for valid points


        //If you hit the ceiling how much are you subtracting from the height of the downwards raycast

        RaycastHit ceilingCheck;
        Vector3 ceilingPoint = Vector3.zero;
        float searchDistanceDrop = E_Demon_Ground.agentSettings[eg.agentIndex].ledgeDropHeight;

        if (Physics.Raycast (transform.position, Vector3.up, out ceilingCheck, E_Demon_Ground.agentSettings[eg.agentIndex].maxJumpAcrossDistance, LayerMask.GetMask ("Ground")))
        {
            //Debug.Log ("Raycast hit something");
            //Debug.Log (ceilingCheck.point);
            //searchDistanceJump = ceilingCheck.point.y;

            ceilingPoint = ceilingCheck.point;
        }
        else
        {
            //Debug.Log ("Raycast did not hit something");
            //Debug.Log (ceilingCheck.point);
            //searchDistanceJump = transform.position.y + Enemy.agentSettings[e.agentIndex].maxJumpAcrossDistance;

            ceilingPoint = transform.position + new Vector3 (0, E_Demon_Ground.agentSettings[eg.agentIndex].maxJumpAcrossDistance, 0);
        }




        //We have the wander offset now. Search this area for the first candidate point in that area.
        for (int i = 0; i < wanderSearchIterations; i++)
        {
            //point = wanderOffset + UnityEngine.Random.insideUnitSphere.normalized * wanderSearchRadius;

            Vector3 point = Vector3.zero;
            Vector2 pointFlat = UnityEngine.Random.insideUnitCircle.normalized * wanderSearchRadius;
            //point = wanderOffset + new Vector3 (pointFlat.x, transform.position.y, pointFlat.y);
            point = wanderOffset + new Vector3 (pointFlat.x, ceilingPoint.y, pointFlat.y);

            //pointSearchOrigin = new Vector3 (point.x, ceilingPoint.y, point.y);

            //Debug.Log ((point.y - transform.position.y) + searchDistanceDrop);
            RaycastHit groundHit;
            //From the point, 
            if (Physics.Raycast (point, Vector3.down, out groundHit, (point.y - transform.position.y) + searchDistanceDrop, LayerMask.GetMask ("Ground")))
            {
                //Debug.Log ("Raycastpoint found");
                //Debug.Log (groundHit.point);
                //Debug.DrawLine (point, new Vector3 (point.x, transform.position.y - searchDistanceDrop, point.z), Color.black);


                //Debug.Log ("Sampling point" + point);


                debugPoint = eg.agentPath.corners[1];
                debugPointCorner = wanderOffset;
                debugPointPlayer = transform.position;
                debugPointGround = groundHit.point;


                //Debug.DrawLine (pointSearchOrigin, new Vector3 (pointSearchOrigin.x, 0, pointSearchOrigin.z), Color.white);
                NavMeshHit navHit;

                //If a point on the navmesh was found
                if (NavMesh.SamplePosition (groundHit.point, out navHit, 1, NavMesh.AllAreas))
                {
                    Debug.Log ("Navmesh wander point found");

                    wanderResult = navHit.position;
                    debugPointSearch = navHit.position;


                    return true;
                }
            }
            else
            {
                Debug.Log ("Raycastpoint not found");

            }
        }

        //if none of the points in the navmesh search were valid:

        Debug.Log ("After searching, wander did not find any points");
        wanderResult = Vector3.zero;
        return false;
    }


    protected bool isWaiting = false;
    protected IEnumerator WanderTimer ()
    {
        eg.animator.Play (animationEnter);

        Debug.Log ("Wander timer enabled");
        isWaiting = true;
        yield return new WaitForSeconds (Random.Range (wanderStayTimeMin, wanderStayTimeMax));

        if (SetWanderPoint ())
        {
            //eg.agent.SetDestination (wanderResult);
            //eg.animator.Play (animationWalk);

            StartCoroutine (MoveToPoint (wanderResult, animationWalk));
        }

        isWaiting = false;
    }

    protected override void OnDestinationReached ()
    {
        StartCoroutine (WanderTimer ());
    }

    protected override void OnDestinationFailed ()
    {
        Debug.LogWarning ($"{name} set an invalid destination while wandering", gameObject);
    }
}
