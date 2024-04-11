using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates If the player is on the ground and moving towards the demon when the trigger is entered
/// Uses the dot product between the player's velocity 
/// </summary>
public class Sensor_DodgeRadius : Sensor
{
    [Tooltip("How close will the player have to pass by the demon to trigger a dodge?")]
    [Range(0f, 1f)]
    public float dodgeRequiredAlignment = 1;

    [Tooltip("How fast does the player have to be going to trigger the dodge?")]
    [Min(0)]
    public float dodgeRequiredSpeed = 0;

    private void OnTriggerEnter (Collider other)
    {
        SetPlayerReference p = other.transform.parent.parent.GetComponent<SetPlayerReference>();

        if ( other.CompareTag ("Player") && p != null)
        {
            Rigidbody prb = p.GetComponent<Rigidbody>();    
            Debug.Log($"Alignment: {Vector3.Dot(prb.velocity.normalized, (transform.position - p.transform.position).normalized)}");
            Debug.Log ($"Speed: {prb.velocity.magnitude}");

            if ( prb.velocity.magnitude > dodgeRequiredSpeed
                && Vector3.Dot (prb.velocity.normalized, (transform.position - p.transform.position).normalized) > dodgeRequiredAlignment
                )
            {
                Debug.Log ("Conditions met");
                Activate ();
            }
            else Debug.Log ("Conditions not met");
        }
    }
}
