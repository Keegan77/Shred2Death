using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Enemy bullets are physics-based projectiles spawned by the enemy class.
/// 
/// </summary>

public class Enemy_Bullet : MonoBehaviour
{
    #region COMPONENTS
    Rigidbody rb;
    #endregion

    //Returns a point in space of which a projectile at its speed
    //will make contact with a player at their current speed.

    //TODO: The slope based off the player's velocity would be a good thing to look at for more accurate shots
    public static Vector3 LeadShot (GameObject p, GameObject e, Enemy_BulletInfo i)
    {
        Rigidbody prb = p.transform.parent.GetComponent<Rigidbody> ();
        if (prb == null) return Vector3.zero;

        if ( prb.velocity == Vector3.zero ) return prb.transform.position + UnityEngine.Random.insideUnitSphere * i.deviation; ;
        //return prb.velocity * ((p.transform.position - e.transform.position).magnitude / speed);

        //Forums approach: Not mathmatecally accurate but it gets close
        //https://forum.unity.com/threads/leading-a-target.193445/

        float distance = Vector3.Distance (e.transform.position, p.transform.position);
        float travelTime = distance / i.speed;


        Vector3 intersect = p.transform.position + prb.velocity.normalized * 5;
        Vector3 intersect2 = p.transform.position + prb.velocity.normalized * 10;
        


        float tp1 = Vector3.Distance(p.transform.position, intersect) / prb.velocity.magnitude;
        float te1 = Vector3.Distance (e.transform.position, intersect) / i.speed;

        float tp2 = Vector3.Distance (p.transform.position, intersect2) / prb.velocity.magnitude;
        float te2 = Vector3.Distance (e.transform.position, intersect2) / i.speed;

        

        //Debug.Log ($"Time 0 || Player: 0 | Bullet: {travelTime}");
        //Debug.Log ($"Time 1 || Player: {tp1} | Bullet: {te1}");
        //Debug.Log ($"Time 2 || Player: {tp2} | Bullet: {te2}");
        

        float slopeP = (tp2 - tp1) / 5;
        float slopeE = (te2 - te1) / 5;

        if (slopeE > slopeP )
        {
            //Debug.Log ("Did not fire. Player is outrunning the bullet.");
            return Vector3.zero;
        }

        float compensate = travelTime / (slopeP - slopeE);

        Vector3 intersect3 = p.transform.position + prb.velocity.normalized * compensate;

        float tp3 = Vector3.Distance (p.transform.position, intersect3) / prb.velocity.magnitude;
        float te3 = Vector3.Distance (e.transform.position, intersect3) / i.speed;

        //Debug.Log ($"Compensation: {compensate}");
        //Debug.Log ($"Time 3 || Player: {tp3} | Bullet: {te3}");

        return intersect3 + UnityEngine.Random.insideUnitSphere * i.deviation;

    }

    private void Awake ()
    {
        rb = GetComponent<Rigidbody> ();

        gameObject.SetActive (false);
    }

    private void OnCollisionEnter (Collision collision)
    {
        Destroy (gameObject);
    }

    private void OnTriggerEnter (Collider other)
    {
        
    }

    private void FixedUpdate ()
    {
        
    }
    
    public IEnumerator lifeTimer (float t)
    {
        yield return new WaitForSeconds (t);
        Destroy (gameObject);
    }
}
