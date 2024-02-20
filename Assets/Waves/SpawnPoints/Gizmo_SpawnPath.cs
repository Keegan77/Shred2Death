using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Attached to spawnPoints. 
/// Flying enemies will use the path provided by the
/// </summary>
/// 

public class Gizmo_SpawnPath : MonoBehaviour
{
    public bool drawOffSelected = false;
    private void Start ()
    {
        this.enabled = false;
    }

    private void OnDrawGizmos ()
    {
        #region Should the gizmo draw?
        //If something is selected
        if ( Selection.activeGameObject != null )
        {
            //Draw: drawOffSelected enabled, it's a child of this object, or it is this object
            if (
                !(drawOffSelected
                || Selection.activeGameObject == gameObject
                || Selection.activeGameObject.transform.IsChildOf (transform))
                ) return;
        }
        //If nothing is selected
        else
        {
            if ( !drawOffSelected ) { return; }
        }

        #endregion

        if ( enabled )
        {
            //Draw line from spawn point to entry point
            Gizmos.DrawLine (transform.position, transform.GetChild (0).position);


            //Draw line from spawn point to first child of the path
            Gizmos.DrawLine (transform.position, transform.GetChild (0).GetChild (0).position);

            //Draw lines along the path of nodes
            for ( int i = 0 ; i < transform.GetChild (0).childCount - 1 ; i++ )
            {
                Gizmos.DrawLine (transform.GetChild (0).GetChild (i).position, transform.GetChild (0).GetChild (i + 1).position);
            }

            //Draw line from last child of path to end point
            Gizmos.DrawLine (transform.GetChild (0).GetChild (transform.GetChild (0).childCount - 1).position, transform.GetChild (0).position);
        }

    }
}
