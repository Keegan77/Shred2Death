using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor_KillOnTouch : Sensor
{
    private void OnTriggerEnter (Collider other)
    {
        
        if (other.CompareTag ("Player"))
        {
            transform.parent.parent.GetComponent<Enemy> ().takeDamage ();
        }
    }
}
