using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Interfaces;
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

    [Tooltip ("How much damage does the bullet do on impact?")]
    public int damage;


    [Tooltip ("When targeting the player, aim for a point in this radius around them.")]
    public float deviation = 1;

    [Tooltip ("Bullet will be cleared after this many seconds of existing and not hitting something")]
    public float timeToLive = 5;

    [SerializeField, Tooltip ("Which sound effects play when the bullet is fired?")]
    AudioClip[] audioBulletStart;

    [SerializeField, Tooltip ("Which sound effects play when the bullet is fired?")]
    AudioClip[] audioBulletTravel;

    [SerializeField, Tooltip ("Which sound effects play when the bullet is fired?")]
    AudioClip[] audioBulletEnd;


    [Header ("Components")]
    [SerializeField]
    Enemy_AudioPlayer audioPlayer;

    [SerializeField]
    AudioSource constantSound;

    [SerializeField]
    GameObject bulletBody;

    private void Awake ()
    {
        gameObject.SetActive (false);
        
    }

    public virtual void StartBullet ()
    {
        gameObject.SetActive (true);
        StartCoroutine (lifeTimer (timeToLive));
        audioPlayer.playClipRandom (audioBulletStart);
    }
    
    public IEnumerator lifeTimer (float t)
    {
        yield return new WaitForSeconds (t);
        Destroy (gameObject);
    }

    private void OnTriggerEnter (Collider other)
    {
        Debug.Log ("<color=red>Bullet</color> Collided with" + other.name, other.gameObject);
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

        StopAllCoroutines ();


        //GetComponent<Rigidbody> ().velocity = Vector3.zero;
        GetComponent<Rigidbody> ().Sleep();

        //Destroy the bullet
        StartCoroutine (destroyAfterSound());
    }

    IEnumerator destroyAfterSound ()
    {
        constantSound.Pause ();

        audioPlayer.playClipRandom (audioBulletEnd);
        bulletBody.SetActive (false);

        yield return new WaitUntil(() => audioPlayer.audioSource.isPlaying == false);

        Destroy (gameObject);
        
    }
}
