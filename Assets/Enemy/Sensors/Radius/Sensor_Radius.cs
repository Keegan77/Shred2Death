using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor_Radius : Sensor
{
    private void OnTriggerEnter (Collider other)
    {
        Debug.Log ($"{other.gameObject.name} entered", gameObject);
        if (other.CompareTag ("Player"))
        {
            Activate ();
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.CompareTag ("Player"))
        {
            Deactivate ();
        }
    }
}
