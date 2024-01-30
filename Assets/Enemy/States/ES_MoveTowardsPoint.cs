using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_MoveTowardsPoint : Enemy_State
{
    [NonSerialized] public GameObject travelPoint;
    public override void Enter ()
    {
        Debug.Log (gameObject + "Entering the arena.");
    }

    public override void machinePhysics ()
    {
        Vector3 pointA = new Vector3 (travelPoint.transform.position.x, 0, travelPoint.transform.position.z);
        Vector3 pointB = new Vector3 (transform.position.x, 0, transform.position.z);

        Vector3 direction = (pointA - pointB).normalized * e.movementSpeed;

        //if you're not there yet, keep moving towards the entry point
        if ((pointA - pointB).magnitude > 1)
        {
            Vector3 newDirection = new Vector3
            (
            direction.x,
            rb.velocity.y,
            direction.y
            );

            rb.velocity = newDirection;
        }

        //Enemy is close enough to the entry point, change state to idle
        else
        {
            transform.parent.GetComponent<Enemy_StateMachine> ().transitionState (GetComponent<ES_Idle> ());
        }
    }
}
