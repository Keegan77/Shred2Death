using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flying : Enemy
{
    [HideInInspector] public Sensor_Spatial s_Spatial;

    private void Awake()
    {
        EnemyGetComponentReferences();

    }

    protected override void EnemyGetComponentReferences()
    {
        base.EnemyGetComponentReferences();

        s_Spatial = transform.Find("Sensors/SpatialOrientation").GetComponent<Sensor_Spatial>();
    }
}
