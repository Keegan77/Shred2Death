using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBP_SingleShot : Enemy_BulletPattern
{
    public float cooldown = 1;
    public override IEnumerator PlayShot (Vector3 target, GameObject muzzle)
    {
        bulletReady = false;

        spawnBullet (target, muzzle);


        yield return new WaitForSeconds (cooldown);
        bulletReady = true;
    }

}
