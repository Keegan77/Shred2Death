using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBP_SingleShot : Enemy_BulletPattern
{
    public float cooldownMin = 1;
    public float cooldownMax = 3;
    public override IEnumerator PlayShot (Vector3 target, GameObject muzzle)
    {
        Debug.Log ($"{name}: Playing shot");

        bulletReady = false;

        spawnBullet (target, muzzle);


        yield return new WaitForSeconds (Random.Range(cooldownMin, cooldownMax));
        bulletReady = true;
    }

}
