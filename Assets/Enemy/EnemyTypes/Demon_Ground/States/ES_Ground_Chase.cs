using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ES_Ground_Chase : ES_DemonGround
{
    const string animationFireball = "FIREBALL";

    [Header ("Navigation")]

    [Tooltip ("How often does navigation update? Lower = more often")]
    [SerializeField] float agentUpdateDistance = 4;

    [Tooltip ("How often will the agent check to see if it should enter turret mode?")]
    [SerializeField] float chaseUpdateTimer = 3;
    bool chaseKey = true;


    [Header ("Projectile")]
    [SerializeField] Enemy_BulletPattern bulletInfo;

    [Tooltip("Wait at least this long to shoot a bullet")]
    [SerializeField] float bulletWaitMin = 1;

    [Tooltip("Wait at most this long to shoot a bullet")] 
    [SerializeField] float bulletWaitMax = 5;

    private bool readyToBullet = false;

    public override void Enter ()
    {
        base.Enter ();
        eg.agent.SetDestination (Enemy.playerReference.transform.position);

        chaseKey = true;

        StartCoroutine (bulletWait ());
    }
    public override void Exit ()
    {
        base.Exit ();
        eg.agent.isStopped = false;
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
        Vector3 playerDestinationOffset = Enemy.playerReference.transform.position - eg.agent.destination;


        if (constantUpdate)
        {
            eg.agent.SetDestination (Enemy.playerReference.transform.position);

            //Debug.Log (e.agent.pathStatus);
        }
        else if (playerDestinationOffset.magnitude > agentUpdateDistance)
        {
            eg.agent.SetDestination (Enemy.playerReference.transform.position);
            Debug.Log ("Resetting Path");
        }

        if (!isAnimationPlaying)
        {
            if (chaseKey)
            {
                if (!Enemy.playerReference.isOnNavMesh)
                {
                    eg.stateMachine.transitionState (GetComponent<ES_Ground_Turret> ());
                    return;
                }

                StartCoroutine (chaseWait ());
            }

            //If a bullet is ready and the enemy has waited long enough, fire a bullet.
            if (bulletInfo.bulletReady && readyToBullet)
            {
                eg.agent.isStopped = true;
                bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject);
                eg.animator.Play (bulletInfo.attackAnimation);
                StartCoroutine (bulletWait ());
            }

        }
    }

    IEnumerator chaseWait ()
    {
        chaseKey = false;

        yield return new WaitForSeconds (chaseUpdateTimer);

        chaseKey = true;
    }

    IEnumerator bulletWait ()
    {
        readyToBullet = false;

        yield return new WaitForSeconds (Random.Range(bulletWaitMin, bulletWaitMax));

        readyToBullet = true;
    }

    protected override void OnDestinationReached ()
    {
        Debug.Log ($"{gameObject.name}: Melee the player!");

        eg.stateMachine.transitionState(GetComponent<ES_Ground_Turret> ());

    }
    protected override void OnDestinationFailed ()
    {
        throw new System.NotImplementedException ();
    }

    public override void OnBullet ()
    {
        bulletInfo.StartCoroutine (bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject));
    }

    public override void OnAnimationFinished ()
    {
        base.OnAnimationFinished ();
        eg.agent.isStopped = false;
        eg.animator.Play (animationEnter);
    }
}
