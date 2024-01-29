using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerReference : MonoBehaviour
{
    //Enemy states need to know what the player object is for directional purposes.
    //This sets the enemy_state script up for that when the player loads into the level.
    void Start()
    {
        Enemy_State.playerObject = gameObject;
    }
}
