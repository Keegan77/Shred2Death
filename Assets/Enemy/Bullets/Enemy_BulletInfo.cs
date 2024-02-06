using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BulletInfo : MonoBehaviour
{
    #region PARAMETERS
    public GameObject bulletObject;

    public float speed = 15;
    public float fireRate = 1;
    public float timeToLive = 5;

    [Tooltip("When targeting the player, aim for a point in this radius around them.")]
    public float deviation = 0;

    #endregion

    public void spawnBullet(GameObject muzzle)
    {
        Vector3 solvedPosition = Enemy_Bullet.LeadShot(Enemy.playerObject, muzzle, this);
        //Debug.Log(solvedPosition);
        if(solvedPosition == Vector3.zero)
        {
            Debug.Log ("Recognized bullet won't fire right");
            return;
        }

        GameObject eb = Instantiate (bulletObject, muzzle.transform.position, Quaternion.identity);

        //Once the bullet is spawned,
        eb.transform.LookAt (solvedPosition);

        eb.GetComponent<Rigidbody> ().velocity = eb.transform.forward * speed;

        eb.SetActive (true);

        eb.GetComponent<Enemy_Bullet>().StartCoroutine(eb.GetComponent<Enemy_Bullet>().lifeTimer(timeToLive));

        #region debug lines
        //Debug.DrawLine(Enemy.playerObject.transform.position, solvedPosition);
        //Debug.DrawLine(Enemy.playerObject.transform.position + new Vector3 (0, 1, 0), ((solvedPosition - Enemy.playerObject.transform.position).normalized) * Enemy.playerObject.GetComponent<Rigidbody>().velocity.magnitude + new Vector3 (0, 1, 0) + Enemy.playerObject.transform.position);
        //Debug.DrawLine(Enemy.playerObject.transform.position + new Vector3 (0, 2, 0), ((solvedPosition - Enemy.playerObject.transform.position).normalized) * Enemy.playerObject.GetComponent<Rigidbody>().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + Enemy.playerObject.transform.position);

        //Debug.DrawLine(eb.transform.position, solvedPosition);
        //Debug.DrawLine (eb.transform.position + new Vector3 (0, 1, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude + new Vector3 (0, 1, 0) + eb.transform.position);
        //Debug.DrawLine (eb.transform.position + new Vector3 (0, 2, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + eb.transform.position);
        #endregion

    }
}
