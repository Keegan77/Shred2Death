using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SetPlayerReference : MonoBehaviour
{

    [Tooltip("How often does the player sample the navmesh to see if they're on it?")]
    public float navMeshSampleRate = 1.0f;
    private bool sampleAvailable = false;

    public bool isOnNavMesh = false;

    //Enemy states need to know what the player object is for directional purposes.
    //This sets the enemy_state script up for that when the player loads into the level.
    
    void Start()
    {
        Enemy.playerObject = gameObject;
    }

    private void FixedUpdate ()
    {
        
    }

    public bool SampleIsOnNavMesh ()
    {
        return false;
    }

    IEnumerator timer (float t)
    {
        yield return new WaitForSeconds (t);
    }
}
