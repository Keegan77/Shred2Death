using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemies in the chase state move towards the player continuously.
/// Occasionally they stop for a brief second to throw a fireball at the player.
/// 
/// If the player is in the air or the enemy is close enough, enter the turret state.
/// </summary>
public class ESG_Chase : ES_DemonGround
{
    [Header ("Navigation")]

    [Tooltip ("How often does navigation update? Lower = more often")]
    [SerializeField] float agentUpdateDistance = 4;


    [Header ("Projectile")]
    [SerializeField] Enemy_BulletPattern bulletInfo;

    private bool readyToBullet = true;

    public override void Enter ()
    {
        base.Enter ();
        eg.agent.SetDestination (Enemy.playerReference.transform.position);

        //StartCoroutine (bulletWait ());
    }
    public override void Exit ()
    {
        base.Exit ();
        eg.agent.isStopped = false;

        bulletInfo.CancelShot ();
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
       
    }


    public override void AIUpdate ()
    {
        Vector3 playerDestinationOffset = Enemy.playerReference.transform.position - eg.agent.destination;

        if ( eg.isInMeleeRange )
        {
            eg.stateMachine.transitionState (GetComponent<ESG_Turret> ());
            return;
        }

        //Did the player move far enough away from the previous destination?
        if (playerDestinationOffset.magnitude > agentUpdateDistance)
        {
            eg.agent.SetDestination (Enemy.playerReference.transform.position);
            Debug.Log ("Resetting Path");
        }

        if ( !isAnimationPlaying )
        {

            if ( !Enemy.playerReference.isOnNavMesh )
            {
                eg.stateMachine.transitionState (GetComponent<ESG_Turret> ());
                return;
            }

            //If a bullet is ready and the enemy has waited long enough, fire a bullet.
            if ( bulletInfo.bulletReady && readyToBullet )
            {
                Debug.Log ("EnemyPlayingShot On The Run");
                //eg.agent.isStopped = true;

                bulletInfo.StartCoroutine (bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject));
                //eg.animator.Play (bulletInfo.attackAnimation);
                //StartCoroutine (bulletWait ());
            }

        }
    }

    protected override void OnDestinationReached ()
    {
        Debug.Log ($"{gameObject.name}: Melee the player!");

        eg.stateMachine.transitionState(GetComponent<ESG_Turret> ());

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
