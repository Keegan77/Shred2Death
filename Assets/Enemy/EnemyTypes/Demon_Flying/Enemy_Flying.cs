using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flying : Enemy
{
    [HideInInspector] public Sensor_Spatial s_Spatial;

    [Header ("Movement")]
    public float movementSpeed = 1.0f;
    public float movementSpeedShift = 1.0f;

    public float movementStoppingDistance = 1.0f;

    [Tooltip("How many degrees per second does the enemy turn?")]
    public float movementTurnSpeed= 1f;

    private void Awake()
    {
        EnemyGetComponentReferences();

    }

    protected override void EnemyGetComponentReferences()
    {
        base.EnemyGetComponentReferences();

        s_Spatial = transform.Find("Sensors/SpatialOrientation").GetComponent<Sensor_Spatial>();
    }

    public override void TrickOffEvent (Vector3 playerVel)
    {
        stateMachine.statesObject.GetComponent<ES_Ragdoll> ().EnterRagdoll (new Vector3 (0, -5, 0), true);
    }
}
