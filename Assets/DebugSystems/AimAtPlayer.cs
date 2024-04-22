using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows me to have an object that can be placed on something and have it shoot at the player.
/// </summary>
public class AimAtPlayer : MonoBehaviour
{
    [SerializeField]
    Enemy_BulletPattern pattern;

    public void fireBullet ()
    {
        StartCoroutine (pattern.PlayShot (Enemy.playerReference.gameObject, Enemy.playerReference.rb, gameObject));
    }
}
