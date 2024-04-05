using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Spawns a single bullet that leads the player by their linear speed.
/// </summary>
public class EBP_SingleShot : Enemy_BulletPattern
{
    public override IEnumerator PlayShot (GameObject target, GameObject muzzle)
    {

        yield return new WaitForSeconds (timeLeadIn);
        bulletPlaying = true;
        bulletReady = false;

        //LeadShot(target, muzzle, bulletObject)
        SpawnBullet (LeadShot(target, muzzle, bulletObject), muzzle);


        yield return new WaitForSeconds (timeLeadOut - timeLeadIn);
        bulletReady = true;
        bulletPlaying = false;
    }

}
