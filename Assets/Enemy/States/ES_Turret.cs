using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// In this state an enemy will sit still and fire bullets towards the player
/// using the stats provided by the selected BulletInfo
/// </summary>
public class ES_Turret : Enemy_State
{
    #region PARAMETERS
    [SerializeField] Enemy_BulletInfo bulletInfo;

    bool bulletReady = true;
    #endregion


    public override void Enter ()
    {
        base.Enter ();
        bulletReady = true;
    }

    public override void machinePhysics ()
    {
        if ( bulletReady )
        {
            StartCoroutine (fireBullet());
        }       
    }

    IEnumerator fireBullet ()
    {
        bulletReady = false;
        yield return new WaitForSeconds (1 / bulletInfo.fireRate);
        
        NavMeshHit hit;
        if ( NavMesh.SamplePosition (Enemy.playerObject.transform.position, out hit, 2, NavMesh.AllAreas) )
        {
            e.stateMachine.transitionState (GetComponent<ES_Chase> ());
        }
        else
        {
            e.spawnBullet (bulletInfo);
        }

        bulletReady = true;
    }
}
