using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
public class SetPlayerReference : MonoBehaviour
{

    [HideInInspector] public bool isOnNavMesh = false;
    NavMeshHit hit;

    [NonSerialized] public Vector3 aimOffset = new Vector3 (0, 2, 0);

    //Enemy states need to know what the player object is for directional purposes.
    //This sets the enemy_state script up for that when the player loads into the level.
    
    void Start()
    {
        Enemy.playerReference = this;
    }

    private void FixedUpdate ()
    {
        isOnNavMesh = SampleIsOnNavMesh ();
    }

    private bool SampleIsOnNavMesh ()
    {
        return NavMesh.SamplePosition(transform.position, out hit, 2, NavMesh.AllAreas);
    }

}
