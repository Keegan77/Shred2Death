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
    private void OnTriggerEnter (Collider other)
    {
        Activate ();

        if (other.CompareTag ("Player"))
        {
            playerHit.Invoke ();
        }
    }
}
