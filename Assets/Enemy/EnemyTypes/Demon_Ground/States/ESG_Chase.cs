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
    [SerializeField] ES_Attack stateAttack;
    [SerializeField] float timerShotSetting = 3;

    public override void Enter ()
    {
        base.Enter ();
        eg.agent.SetDestination (Enemy.playerReference.transform.position);

    }
    public override void Exit ()
    {
        base.Exit ();
    }

    public override void onPlayerSensorDeactivated ()
    {
        //transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (returnState);
    }

    public override void onPlayerSensorActivated ()
    {
    }

    public override void machinePhysics ()
    {
       
    }
    public override void machineUpdate ()
    {
        
    }


    public override void AIUpdate ()
    {
        Vector3 playerDestinationOffset = Enemy.playerReference.transform.position - eg.agent.destination;

        if ( eg.isInMeleeRange )
        {
            Debug.Log ("YIPEEEEEEE");
            eg.stateMachine.transitionState (GetComponent<ESG_Turret> ());
            return;
        }

        //Did the player move far enough away from the previous destination?
        if (playerDestinationOffset.magnitude > agentUpdateDistance)
        {
            eg.agent.SetDestination (Enemy.playerReference.navMeshPing.position);
            //Debug.Log ("Resetting Path");
        }

        if ( !isAnimationPlaying )
        {

            if ( !Enemy.playerReference.isOnNavMesh )
            {
                eg.stateMachine.transitionState (GetComponent<ESG_Turret> ());
                return;
            }

            //If a bullet is ready and the enemy has waited long enough, fire a bullet.
            if ( stateAttack.bulletInfo.bulletReady && e.stateMachine.timerCurrentState > timerShotSetting )
            {
                //Debug.Log ("EnemyPlayingShot On The Run");
                e.stateMachine.transitionState(stateAttack);
                

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
        Debug.LogWarning ("Animation tried to call obselete OnBullet");
        //stateAttack.bulletInfo.StartCoroutine (stateAttack.bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject));
    }

    public override void OnAnimationFinished ()
    {
        base.OnAnimationFinished ();
        eg.agent.isStopped = false;
        eg.animator.Play (animationEnter);
    }
}
