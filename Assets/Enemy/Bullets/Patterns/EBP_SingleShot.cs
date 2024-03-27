using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Spawns a single bullet that leads the player by their linear speed.
/// </summary>
public class EBP_SingleShot : Enemy_BulletPattern
{
    public float cooldownMin = 1;
    public float cooldownMax = 3;

    
    public override IEnumerator PlayShot (GameObject target, GameObject muzzle)
    {
        bulletReady = false;
        tokens -= tokenCost;

        //LeadShot(target, muzzle, bulletObject)
        SpawnBullet (LeadShot(target, muzzle, bulletObject), muzzle);

        yield return new WaitForSeconds (Random.Range(cooldownMin, cooldownMax));
        bulletReady = true;
        tokens += tokenCost;
    }

}
