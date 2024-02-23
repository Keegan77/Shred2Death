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
        bulletReady = true;
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
            e.stateMachine.transitionState (GetComponent<ES_Ground_Chase> ());
        }
        else
        {
            bulletInfo.spawnBullet (Enemy.playerObject.transform.position, e.muzzleObject);
        }

        bulletReady = true;
    }
}
