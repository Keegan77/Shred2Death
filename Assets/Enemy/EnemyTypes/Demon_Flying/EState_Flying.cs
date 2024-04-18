using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Flying states are for enemies that navigate straight to the player, without a navmesh.
/// This type of state references a spatial sensor on the enemy as well as functions for moving around using it.
/// 
/// Scripts that utilize this functionality are abbreviated ESF_
/// </summary>
public class EState_Flying : Enemy_State
{
    protected Enemy_Flying eFly;

    protected Vector3 movementAvoidance = Vector3.zero;
    protected Vector3 movementDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake ()
    {
        eFly = transform.parent.GetComponent<Enemy_Flying>();
    }

    /// <summary>
    /// Used by MoveToPoint to determine if the destination has been reached.
    /// </summary>
    /// <returns>True if the enemy is close enough to a stopping point based on Flying Enemy stopping distance</returns>
    bool isAtPoint (GameObject p)
    {
        return Vector3.Distance (transform.position, p.transform.position) <= eFly.movementStoppingDistance;
    }

    /// <summary>
    /// Move towards the target point
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected IEnumerator MoveToObject (GameObject p)
    {
        RaycastHit hit;
        while (!isAtPoint (p))
        {
            eFly.stateMachine.travelPoint = p.transform.position;

            //transform.parent.LookAt(e.stateMachine.travelPoint);
            transform.parent.LookAt(new Vector3(p.transform.position.x, transform.position.y, p.transform.position.z));

            movementAvoidance = Vector3.MoveTowards (
                    movementAvoidance,
                    eFly.s_Spatial.pingResult.normalized,
                    eFly.movementSpeedShift * Time.deltaTime
                    );

            movementDirection = Vector3.MoveTowards 
                (
                movementDirection,
                (p.transform.position - transform.position).normalized,
                eFly.movementSpeedShift * Time.deltaTime
                );

            eFly.rb.velocity = (movementDirection.normalized + movementAvoidance) * eFly.movementSpeed;

            //Check to see if the path to the destination is blocked.
            //If it is, move in that direction but with obstacle avoidance.
            if ( Physics.SphereCast (
                    transform.position,
                    1,
                    p.transform.position - transform.position,
                    out hit,
                    eFly.s_Spatial.sensorLength,
                    eFly.s_Spatial.maskRaycast
                    )
                )
            {
                eFly.s_Spatial.updateSpatialSensor ();
            }
            else
            {
                eFly.s_Spatial.pingResult = Vector3.zero;
            }

            Debug.DrawLine (
            transform.position,
            transform.position + (eFly.stateMachine.travelPoint - transform.position).normalized * eFly.s_Spatial.sensorLength,
            Physics.Raycast (transform.position, eFly.stateMachine.travelPoint - transform.position, eFly.s_Spatial.sensorLength, eFly.s_Spatial.maskRaycast) ? Color.red : Color.white
            );

            Debug.DrawLine (transform.position, transform.position + movementAvoidance, Color.green);



            yield return null;
        }

        onPointReached ();
        Debug.Log ("Arrived at point", gameObject);
    }

    protected IEnumerator MoveThroughPath (GameObject [] points)
    {
        foreach (GameObject g in points)
        {
            yield return MoveToObject (g);
        }

        onPathComplete ();
        Debug.Log ("Path complete", gameObject);
    }

    protected virtual void onPointReached ()
    {

    }

    protected virtual void onPathComplete ()
    {

    }
}
