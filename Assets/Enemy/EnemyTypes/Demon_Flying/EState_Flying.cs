using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EState_Flying : Enemy_State
{
    protected Enemy_Flying e;

    protected Vector3 movementAvoidance = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy_Flying>();
    }

    /// <summary>
    /// Used by MoveToPoint to determine if the destination has been reached.
    /// </summary>
    /// <returns>True if the enemy is close enough to a stopping point based on Flying Enemy stopping distance</returns>
    bool isAtPoint ()
    {
        return Vector3.Distance (transform.position, e.stateMachine.travelPoint) <= e.s_Spatial.sensorLength;
    }

    /// <summary>
    /// Move towards the target point
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected IEnumerator MoveToObject (GameObject p)
    {
        e.stateMachine.travelPoint = p.transform.position;

        while (!isAtPoint ())
        {
            //transform.parent.LookAt(e.stateMachine.travelPoint);
            transform.parent.LookAt(new Vector3(e.stateMachine.travelPoint.x, transform.position.y, e.stateMachine.travelPoint.z));

            movementAvoidance = Vector3.MoveTowards (
                    movementAvoidance,
                    e.s_Spatial.pingResult.normalized,
                    e.movementSpeedShift * Time.deltaTime
                    );

            e.rb.velocity = ((e.stateMachine.travelPoint - transform.position).normalized + movementAvoidance) * e.movementSpeed;

            //Check to see if the path to the destination is blocked.
            //If it is, move in that direction but with obstacle avoidance.
            if ( Physics.Raycast (
                    transform.position,
                    e.stateMachine.travelPoint - transform.position,
                    e.s_Spatial.sensorLength,
                    e.s_Spatial.maskRaycast
                    )
                )
            {
                e.s_Spatial.updateSpatialSensor ();
            }
            else
            {
                e.s_Spatial.pingResult = Vector3.zero;
            }



            yield return null;
        }

        Debug.Log ("Point has been reached");
        onPointReached ();
    }

    protected virtual void onPointReached ()
    {

    }
}
