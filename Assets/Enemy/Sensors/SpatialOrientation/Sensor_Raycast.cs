using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


/// <summary>
/// Raycast sensors are used by spatial_Sensor through the ping function 
/// </summary>
public class Sensor_Raycast : Sensor
{
    public Sensor_Raycast(float l)
    {
        raycastLength = l;
    }

    public float raycastLength = 5;
    public override bool Ping()
    {
        return Physics.Raycast(transform.parent.position, transform.position, raycastLength);
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, 3 * (transform.position - transform.parent.position) + transform.parent.position);
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0.1f, 0));
    }
}
