using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flying : Enemy
{
    [HideInInspector] public Sensor_Spatial sensorSpatial;

    private void Awake()
    {
        EnemyGetComponentReferences();

        
    }

    protected override void EnemyGetComponentReferences()
    {
        base.EnemyGetComponentReferences();

        sensorSpatial = transform.Find("Sensors/SpatialOrientation").GetComponent<Sensor_Spatial>();
    }
}
