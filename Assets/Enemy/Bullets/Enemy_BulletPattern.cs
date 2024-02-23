using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the first of two systems regarding the firing of bullets.
/// Bullet Patterns are a paradigm of which enemies can spawn bullets by.
/// 
/// Scripts deriving from this are prefixed EBP_
/// </summary>
[Serializable]
public abstract class Enemy_BulletPattern : MonoBehaviour
{
    bool bulletReady = false;

    [Header("Basic Info")]
    public GameObject bulletObject;

    protected abstract IEnumerator playShot ();




    #region Aiming and shooting
    public void spawnBullet (Vector3 target, GameObject muzzle)
    {
        //GameObject eb = Instantiate (bulletObject, muzzle.transform.position, Quaternion.identity);
        GameObject eb = Instantiate (bulletObject, muzzle.transform.position, Quaternion.identity);

        //Once the bullet is spawned,
        eb.transform.LookAt (target);

        eb.GetComponent<Enemy_Bullet> ().StartBullet ();

        #region debug lines
        //Debug.DrawLine(Enemy.playerObject.transform.position, solvedPosition);
        //Debug.DrawLine(Enemy.playerObject.transform.position + new Vector3 (0, 1, 0), ((solvedPosition - Enemy.playerObject.transform.position).normalized) * Enemy.playerObject.GetComponent<Rigidbody>().velocity.magnitude + new Vector3 (0, 1, 0) + Enemy.playerObject.transform.position);
        //Debug.DrawLine(Enemy.playerObject.transform.position + new Vector3 (0, 2, 0), ((solvedPosition - Enemy.playerObject.transform.position).normalized) * Enemy.playerObject.GetComponent<Rigidbody>().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + Enemy.playerObject.transform.position);

        //Debug.DrawLine(eb.transform.position, solvedPosition);
        //Debug.DrawLine (eb.transform.position + new Vector3 (0, 1, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude + new Vector3 (0, 1, 0) + eb.transform.position);
        //Debug.DrawLine (eb.transform.position + new Vector3 (0, 2, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + eb.transform.position);
        #endregion

    }

    #endregion
}
