using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// This is the secnod of two systems regarding the firing of bullets.
/// Bullets are a script attached to the game object
/// that determines how it behaves after being fired by an enemy.
/// 
/// Scripts deriving from this are prefixed EB_
/// </summary>

[RequireComponent (typeof (SphereCollider))]
public abstract class Enemy_Bullet : MonoBehaviour
{
    [Header ("Bullet data")]
    [Tooltip ("How fast does the bullet go?")]
    public float speed = 10;

    [Tooltip("How much damage does the bullet do on impact?")]
    public int damage;


    [Tooltip ("When targeting the player, aim for a point in this radius around them.")]
    public float deviation = 1;

    [Tooltip("Bullet will be cleared after this many seconds of existing and not hitting something")]
    public float timeToLive = 5;

    

    private void Awake ()
    {
        gameObject.SetActive (false);
        
    }

    public void StartBullet ()
    {
        gameObject.SetActive (true);
        StartCoroutine (lifeTimer (timeToLive));
    }
    
    public IEnumerator lifeTimer (float t)
    {
        yield return new WaitForSeconds (t);
        Destroy (gameObject);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Player"))
        {
            if(other.GetComponent<IDamageable>() != null)
            {
                other.GetComponent<IDamageable> ().TakeDamage (damage);
            }
            else
            {
                Debug.LogError ("Player IDamageable Component not found. Damage cannot be taken.");
            }
            
            //throw new NotImplementedException ("No player health script has been linked to enemy bullets");
        }

        //Destroy the bullet
        Destroy(gameObject);
    }
}
