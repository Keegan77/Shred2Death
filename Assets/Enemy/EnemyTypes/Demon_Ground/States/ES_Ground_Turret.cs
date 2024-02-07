using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// In this state an enemy will sit still and fire bullets towards the player
/// using the stats provided by the selected BulletInfo
/// </summary>

public class ES_Ground_Turret : Enemy_State
{
    #region PARAMETERS
    [SerializeField] protected Enemy_BulletInfo bulletInfo;
    #endregion

    public override void Enter ()
    {
        e.bulletReady = true;
    }

    public override void machinePhysics ()
    {
        if (e.bulletReady)
        {
            StartCoroutine (fireBullet ());
        }
    }

    IEnumerator fireBullet ()
    {
        e.bulletReady = false;
        yield return new WaitForSeconds (1 / bulletInfo.fireRate);

        NavMeshHit hit;
        if (NavMesh.SamplePosition (Enemy.playerObject.transform.position, out hit, 4, NavMesh.AllAreas))
        {
            e.stateMachine.transitionState (GetComponent<ES_Chase> ());
        }
        else
        {
            bulletInfo.spawnBullet (e.muzzlePoint);
        }

        e.bulletReady = true;
    }
}
