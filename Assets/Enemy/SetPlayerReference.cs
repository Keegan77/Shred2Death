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

    public bool isOnNavMesh = false;
    public NavMeshHit navMeshPing;

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
        RaycastHit rc;
        //if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.down, out rc, 1000, LayerMask.NameToLayer("Ground")))
        if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.down, out rc, 1000))
        {
            //Debug.Log ("Player is above something");
            //Debug.Log (rc.collider);
            //Debug.Log (rc.collider.name);
            return NavMesh.SamplePosition(rc.point, out navMeshPing, 2, NavMesh.AllAreas);

        }
        else 
        {
            //Debug.Log (rc.collider);
            //Debug.Log ("Player is not on Navmesh"); 
            return false;  
        }
    }

}
