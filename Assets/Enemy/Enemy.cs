using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(Enemy_StateMachine))]
public class Enemy : MonoBehaviour, IDamageable
{
    #region SCRIPT VARIABLES
    #region Game Objects
    public static SetPlayerReference playerReference;

    [HideInInspector] public Rigidbody rb;
   

    [HideInInspector] public Enemy_StateMachine stateMachine;

    [HideInInspector] public GameObject muzzleObject;

    [HideInInspector] public Animator animator;
    #endregion

    #region Enemy Stats
    public int health;
    bool isDead = false;
    #endregion

    [Header ("Components")]
    WaveManager waveManager; //Set by waveManager when the enemy object is instantiated
    
    #endregion

    #region SETUP
    void Awake ()
    {
        EnemyGetComponentReferences();
    }

    /// <summary>
    /// Gets the basic components of the enemy class to reduce boilerplate code. Future iterations of enemies should extend this function as well to provide for the next inherited enemy type.
    /// </summary>
    protected virtual void EnemyGetComponentReferences ()
    {
        stateMachine = GetComponent<Enemy_StateMachine> ();
        rb = GetComponent<Rigidbody> ();

        muzzleObject = transform.Find ("Body/MuzzlePoint").gameObject;
    }

    //After it's spawned, the static variable for agentSettings should exist.
    private void Start ()
    {
        
    }

    public void SetManager (WaveManager w)
    {
        waveManager = w;
    }

    #endregion

    #region SCRIPT FUNCTIONS

    public static void spawnObject ()
    {
        Instantiate (new GameObject ("Statically Spawned"));
    }

    public void TakeDamage (float damage)
    {        
        health -= Mathf.FloorToInt(damage);

        if (health <= 0 && !isDead) 
        {
            isDead = true;

            rb.detectCollisions = false;
            GetComponent<CapsuleCollider> ().enabled = false;

            DissolvingController d = transform.Find("Body").GetComponent<DissolvingController>();

            d.StartCoroutine (d.Dissolve ());
        }
    }

    public void DeathFinished ()
    {
        Debug.Log ("Enemy Dead");

        waveManager.removeEnemy ();
        Destroy (gameObject);
    }

    #endregion

    #region UNITY FUNCTIONS
    void Update ()
    {
        if (stateMachine.enabled)
        {
            stateMachine.machineUpdate();
        }
    }

    private void FixedUpdate ()
    {
        if (stateMachine.enabled)
        {
            stateMachine.machinePhysics();
        }

    }
    #endregion
}
