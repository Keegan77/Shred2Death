using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// This is the secnod of two systems regarding the firing of bullets.
/// Bullets are a script attached to the game object
/// that determines how it behaves after being fired by an enemy.
/// 
/// Scripts deriving from this are prefixed EB_
/// </summary>

[RequireComponent (typeof (SphereCollider))]
public abstract class Enemy_Bullet : MonoBehaviour
{
    [Header ("Bullet data")]
    [Tooltip ("How fast does the bullet go?")]
    public float speed = 10;

    [Tooltip ("When targeting the player, aim for a point in this radius around them.")]
    public float deviation = 1;

    [Tooltip("Bullet will be cleared after this many seconds of existing and not hitting something")]
    public float timeToLive = 5;

    

    private void Awake ()
    {
        gameObject.SetActive (false);
        
    }

    public void StartBullet ()
    {
        gameObject.SetActive (true);
        StartCoroutine (lifeTimer (timeToLive));
    }
    
    public IEnumerator lifeTimer (float t)
    {
        yield return new WaitForSeconds (t);
        Destroy (gameObject);
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
        Enemy_Bullet bullet = bulletObject.GetComponent<Enemy_Bullet>();

        //Get the main player object
        GameObject p2 = target.transform.gameObject;

        Rigidbody prb = p2.GetComponent<Rigidbody> ();
        if ( prb == null ) return Vector3.zero;

        //PlayerBase pb = p2.GetComponent<PlayerBase> ();

        //float movement = pb.movement.turnSharpness * InputRouting.Instance.GetMoveInput ().x * Time.deltaTime;
        //Debug.Log ($"movement: {movement} | Input: {InputRouting.Instance.GetMoveInput ().x} | Turnspeed: {pb.movement.turnSharpness}");

        if ( prb.velocity == Vector3.zero ) return prb.transform.position + UnityEngine.Random.insideUnitSphere * bullet.deviation;
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

        if ( slopeE > slopeP )
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
}
