using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Placeholder script to make the camera follow the player until we get a dedicated script.
/// Follows the player on an X and Y Axis
/// </summary>
public class temp_CamFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    private void Update ()
    {
        transform.position = player.transform.position + offset;
    }
}
