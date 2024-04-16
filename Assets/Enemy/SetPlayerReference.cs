using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script attached to the player 
/// </summary>
public class SetPlayerReference : MonoBehaviour
{
    public PlayerBase playerbase = null;

    [HideInInspector] public bool isOnNavMesh = false;
    NavMeshHit hit;

    //[NonSerialized] public Vector3 aimOffset = new Vector3 (0, 2, 0);
    public GameObject aimTarget;
    public Rigidbody rb;
    
    void Start()
    {
        Enemy.playerReference = this;
        playerbase = GetComponent<PlayerBase>();
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
