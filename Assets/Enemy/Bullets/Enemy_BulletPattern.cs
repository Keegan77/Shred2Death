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
    public bool bulletReady { get; protected set; } = true;
    

    [Header("Basic Info")]
    public GameObject bulletObject;
    public string attackAnimation;

    public abstract IEnumerator PlayShot (GameObject target, GameObject muzzle);

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


    //TODO: The slope based off the player's velocity would be a good thing to look at for more accurate shots
    //TODO: Refactor bullets to replace with bullet patterns. Parameter I should be removed.
    /// <summary>
    /// Calculates the direction the enemy would need to shoot in order to hit a moving target.
    /// The calculation is linear, so most shots will miss the player if they're moving in a parabola.
    /// </summary>
    /// <param name="target">GameObject to fire the bullet at</param>
    /// <param name="muzzle">GameObject the bullet spawns at</param>
    /// <returns> The point in space of which the function thinks it should shoot to hit the player</returns>
    public static Vector3 LeadShot (GameObject target, GameObject muzzle, GameObject bulletObject)
    {
        Enemy_Bullet bullet = bulletObject.GetComponent<Enemy_Bullet> ();

        //Get the main player object
        GameObject p2 = target.transform.gameObject;

        Rigidbody prb = p2.GetComponent<Rigidbody> ();
        if (prb == null) return Vector3.zero;

        //PlayerBase pb = p2.GetComponent<PlayerBase> ();

        //float movement = pb.movement.turnSharpness * InputRouting.Instance.GetMoveInput ().x * Time.deltaTime;
        //Debug.Log ($"movement: {movement} | Input: {InputRouting.Instance.GetMoveInput ().x} | Turnspeed: {pb.movement.turnSharpness}");

        if (prb.velocity == Vector3.zero) return prb.transform.position + UnityEngine.Random.insideUnitSphere * bullet.deviation;
        //return prb.velocity * ((p.transform.position - e.transform.position).magnitude / speed);

        //Forums approach: Not mathmatecally accurate but it gets close
        //https://forum.unity.com/threads/leading-a-target.193445/

        float distance = Vector3.Distance (muzzle.transform.position, prb.transform.position);
        float travelTime = distance / bullet.speed;


        Vector3 intersect = p2.transform.position + prb.velocity.normalized * 5;
        Vector3 intersect2 = p2.transform.position + prb.velocity.normalized * 10;



        float tp1 = Vector3.Distance (p2.transform.position, intersect) / prb.velocity.magnitude;
        float te1 = Vector3.Distance (muzzle.transform.position, intersect) / bullet.speed;

        float tp2 = Vector3.Distance (p2.transform.position, intersect2) / prb.velocity.magnitude;
        float te2 = Vector3.Distance (muzzle.transform.position, intersect2) / bullet.speed;



        //Debug.Log ($"Time 0 || Player: 0 | Bullet: {travelTime}");
        //Debug.Log ($"Time 1 || Player: {tp1} | Bullet: {te1}");
        //Debug.Log ($"Time 2 || Player: {tp2} | Bullet: {te2}");


        float slopeP = (tp2 - tp1) / 5;
        float slopeE = (te2 - te1) / 5;

        if (slopeE > slopeP)
        {
            //Debug.Log ("Did not fire. Player is outrunning the bullet.");
            return Vector3.zero;
        }

        float compensate = travelTime / (slopeP - slopeE);

        Vector3 intersect3 = target.transform.position + prb.velocity.normalized * compensate;

        float tp3 = Vector3.Distance (target.transform.position, intersect3) / prb.velocity.magnitude;
        float te3 = Vector3.Distance (muzzle.transform.position, intersect3) / bullet.speed;

        //Debug.Log ($"Compensation: {compensate}");
        //Debug.Log ($"Time 3 || Player: {tp3} | Bullet: {te3}");

        return intersect3 + UnityEngine.Random.insideUnitSphere * bullet.deviation;

    }

    #endregion
}
