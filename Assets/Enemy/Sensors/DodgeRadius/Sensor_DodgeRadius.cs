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
        if (!other.CompareTag ("Player")) return;

        //Debug.LogWarning (other.GetInstanceID(), other);

        //Debug.Log ($"{transform.parent.name}: {name} Entered");
        SetPlayerReference p = other.GetComponentInParent<SetPlayerReference>();
        E_Demon_Ground eg = transform.GetComponentInParent<E_Demon_Ground> ();
        if (p != null && eg.rb.isKinematic)
        {
            Rigidbody prb = p.GetComponent<Rigidbody>();    
            //Debug.Log($"Alignment: {Vector3.Dot(prb.velocity.normalized, (transform.position - p.transform.position).normalized)}");
            //Debug.Log ($"Speed: {prb.velocity.magnitude}");

            if ( prb.velocity.magnitude > dodgeRequiredSpeed
                && Vector3.Dot (prb.velocity.normalized, (transform.position - p.transform.position).normalized) > dodgeRequiredAlignment
                )
            {
                Activate ();
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (!other.CompareTag ("Player")) return;

        Deactivate ();
    }
}
