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
    [SerializeField] protected ES_Attack stateAttack;
    #endregion


    public override void Enter ()
    {
        base.Enter ();
        eg.agent.ResetPath ();
    }

    public override void Exit ()
    {
        base.Exit ();
    }

    public override void machinePhysics ()
    {
        
    }

    public override void AIUpdate ()
    {
        if (Enemy.playerReference.isOnNavMesh && !eg.isInMeleeRange)
        {
            eg.stateMachine.transitionState (GetComponent<ESG_Chase> ());
            return;
        }
        if ( stateAttack.bulletInfo.bulletReady )
        {
            //FireBullet ();
            e.stateMachine.transitionState (stateAttack);
        }
    }

    public override void OnBullet ()
    {
        Debug.LogWarning ("<color=red>Calling obselete function OnBullet", this);
        //bulletInfo.spawnBullet (Enemy.playerObject.transform.position, eg.muzzleObject);
        //bulletInfo.StartCoroutine (bulletInfo.PlayShot (Enemy.playerReference.gameObject, eg.muzzleObject));
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
