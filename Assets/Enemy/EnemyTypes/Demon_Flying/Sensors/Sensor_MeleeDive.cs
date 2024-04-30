using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Temporary solution to get Melee Attacks working before I separate hitboxes from the bullet system.
/// </summary>
public class Sensor_MeleeDive : Sensor
{
    [SerializeField] UnityEvent playerHit;

    [Min(0)]
    [SerializeField] float damage = 18f;
    private void OnTriggerEnter (Collider other)
    {
        Activate ();

        if (other.CompareTag ("Player"))
        {
            if (other.GetComponent<IDamageable>() != null)
            {
                other.GetComponent<IDamageable> ().TakeDamage (damage);
            }
            else
            {
                Debug.LogWarning ($"{gameObject}: Player iDamageable not found");
            }

            playerHit.Invoke ();
        }
    }
}
