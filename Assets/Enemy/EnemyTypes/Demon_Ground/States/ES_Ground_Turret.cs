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
    [SerializeField] protected Enemy_BulletPattern bulletInfo;
    #endregion

    bool bulletReady = false;

    public override void Enter ()
    {
        base.Enter ();
        bulletReady = true;

        eg.agent.SetDestination (transform.position);
        eg.agent.isStopped = true;

        eg.animator.Play ("FIREBALL");
    }

    public override void machinePhysics ()
    {
        if (bulletReady)
        {
            StartCoroutine (fireBullet ());
        }
    }

    IEnumerator fireBullet ()
    {
        bulletReady = false;
        yield return new WaitForSeconds (1); //deal with fire rate later

        NavMeshHit hit;
        if (NavMesh.SamplePosition (Enemy.playerObject.transform.position, out hit, 4, NavMesh.AllAreas))
        {
            eg.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        }
        else
        {
            bulletInfo.spawnBullet (Enemy.playerObject.transform.position, eg.muzzleObject);
        }

        bulletReady = true;
    }

    protected override void OnDestinationReached ()
    {
        
    }

    protected override void OnDestinationFailed ()
    {
        throw new System.NotImplementedException ();
    }
}
