using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Enemy bullets are physics-based projectiles spawned by the enemy class.
/// 
/// </summary>

[RequireComponent (typeof (SphereCollider))]
public abstract class Enemy_Bullet : MonoBehaviour
{
    [Header ("Bullet data")]
    [Tooltip ("How fast does the bullet go?")]
    public float speed = 10;

    [Tooltip ("When targeting the player, aim for a point in this radius around them.")]
    public float deviation = 1;

    [Tooltip("Bullet will be cleared after this many seconds of existing and not hitting something")]
    public float timeToLive = 5;

    

    private void Awake ()
    {
        gameObject.SetActive (false);
        
    }

    public void Bullet ()
    {
        StartCoroutine (lifeTimer (timeToLive));
        playBullet ();
    }

    protected abstract void playBullet ();
    
    public IEnumerator lifeTimer (float t)
    {
        yield return new WaitForSeconds (t);
        Destroy (gameObject);
    }
}
