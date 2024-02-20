using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Events;


/// <summary>
/// Raycast sensors are used by spatial_Sensor through the ping function 
/// </summary>
public class Sensor_Raycast : Sensor
{
    public RaycastHit hit;
    public Color colorResult = Color.white;

    public float raycastLength = 5;
    public LayerMask maskRaycast;

    /// <summary>
    /// Raycast sensors will cast their parent's position against their position to determine if there has been a collision
    /// </summary>
    /// <returns>True if the raycast collides with something</returns>
    public override bool Ping()
    {
        bool pingResult = Physics.SphereCast
            (
                transform.position,
                1,
                (transform.position - transform.parent.position),
                out hit,
                raycastLength,
                maskRaycast
            );


        colorResult = pingResult ? Color.red : Color.white;

        Debug.DrawLine (
                    transform.position,
                    (transform.position - transform.parent.position).normalized * raycastLength + transform.parent.position,
                    colorResult
                    );

        return pingResult;
    }

    private void Update()
    {
        
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0.1f, 0));
    }
}
