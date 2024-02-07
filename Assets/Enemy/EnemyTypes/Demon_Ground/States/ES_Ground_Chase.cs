using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ES_Ground_Chase : Enemy_State
{
    [Header ("Navigation")]

    [Tooltip ("How often does navigation update? Lower = more often")]
    [SerializeField] float agentUpdateDistance = 4;

    [Tooltip ("How often will the agent check to see if it should enter turret mode?")]
    [SerializeField] float chaseUpdateTimer = 3;
    bool chaseKey = true;

    bool bulletKey = true;



    [Header ("Projectile")]
    [SerializeField] Enemy_BulletInfo bulletInfo;
    [SerializeField] float bulletWaitMin = 1;
    [SerializeField] float bulletWaitMax = 5;

    public override void Enter ()
    {
        e.agent.SetDestination (Enemy.playerObject.transform.position);

        bulletKey = true;
        chaseKey = true;
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
            e.agent.SetDestination (Enemy.playerObject.transform.position);
            Debug.Log ("Resetting Path");
        }

        if (chaseKey)
        {
            StartCoroutine (chaseWait ());
        }

        if (bulletKey)
        {
            StartCoroutine (bulletWait ());
        }
    }

    IEnumerator bulletWait ()
    {
        bulletKey = false;

        yield return new WaitForSeconds (UnityEngine.Random.Range (bulletWaitMin, bulletWaitMax));

        bulletInfo.spawnBullet (e.muzzlePoint);

        bulletKey = true;
    }

    IEnumerator chaseWait ()
    {
        chaseKey = false;

        yield return new WaitForSeconds (chaseUpdateTimer);

        NavMeshHit hit;
        if (!NavMesh.SamplePosition (Enemy.playerObject.transform.position, out hit, 2, NavMesh.AllAreas))
        {
            e.stateMachine.transitionState (GetComponent<ES_Turret> ());
        }

        chaseKey = true;
    }
}
