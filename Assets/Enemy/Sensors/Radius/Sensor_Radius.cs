using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor_Radius : Sensor
{
    private void OnTriggerEnter (Collider other)
    {
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