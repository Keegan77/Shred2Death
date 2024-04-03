using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// In this state an enemy will sit still and fire bullets towards the player
/// using the stats provided by the selected BulletInfo
/// </summary>

public class ESG_Turret : ES_DemonGround
{
    #region PARAMETERS
    const string animationIdle = "IDLE";

    [Header ("Turret")]
    [SerializeField] protected float bulletWaitTimeMin = 0f;
    [SerializeField] protected float bulletWaitTimeMax = 1f;
    [SerializeField] protected Enemy_BulletPattern bulletInfo;
    #endregion


    public override void Enter ()
    {
        base.Enter ();

        eg.agent.isStopped = true;
    }

    public override void Exit ()
    {
        base.Exit ();
        eg.agent.isStopped = false;
    }

    public override void machinePhysics ()
    {
        
    }

    public override void AIUpdate ()
    {
        if ( bulletInfo.bulletReady && !isAnimationPlaying )
        {
            FireBullet ();
        }
    }

    void FireBullet ()
    {
        if (Enemy.playerReference.isOnNavMesh && !eg.isInMeleeRange)
        {
            eg.stateMachine.transitionState (GetComponent<ESG_Chase> ());
        }
        else
        {
            isAnimationPlaying = true;
            eg.animator.Play (bulletInfo.attackAnimation);
        }
    }

    public override void OnBullet ()
    {
        //bulletInfo.spawnBullet (Enemy.playerObject.transform.position, eg.muzzleObject);
        bulletInfo.StartCoroutine (bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject));
    }

    public override void OnAnimationFinished ()
    {
        isAnimationPlaying = false;
        eg.animator.Play (animationIdle);
    }

    protected override void OnDestinationReached ()
    {
        
    }

    protected override void OnDestinationFailed ()
    {
        throw new System.NotImplementedException ();
    }
}
