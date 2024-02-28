using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// In this state an enemy will sit still and fire bullets towards the player
/// using the stats provided by the selected BulletInfo
/// </summary>

public class ES_Ground_Turret : ES_DemonGround
{
    #region PARAMETERS
    const string animationIdle = "IDLE";

    [Header ("Turret")]
    [SerializeField] protected float bulletWaitTimeMin = 0f;
    [SerializeField] protected float bulletWaitTimeMax = 1f;
    [SerializeField] protected Enemy_BulletPattern bulletInfo;
    #endregion

    #region SCRIPT VARIABLES
    bool animationPlaying = false;
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
        if (bulletInfo.bulletReady && !animationPlaying)
        {
            FireBullet ();
        }
    }

    void FireBullet ()
    {
        animationPlaying = true;
        eg.animator.Play (animationEnter);

        NavMeshHit hit;
        if (NavMesh.SamplePosition (Enemy.playerObject.transform.position, out hit, 4, NavMesh.AllAreas))
        {
            eg.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        }
    }

    public override void OnBullet ()
    {
        //bulletInfo.spawnBullet (Enemy.playerObject.transform.position, eg.muzzleObject);
        bulletInfo.StartCoroutine (bulletInfo.PlayShot (Enemy.playerObject.transform.position, eg.muzzleObject));
    }

    public override void OnAnimationFinished ()
    {
        Debug.Log ("Fireball Animation Finished");
        animationPlaying = false;
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
