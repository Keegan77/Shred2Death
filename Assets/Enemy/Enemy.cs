using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(Enemy_StateMachine))]
public class Enemy : MonoBehaviour
{
    #region SCRIPT VARIABLES
    #region Game Objects
    public static GameObject playerObject;

    [NonSerialized] public Rigidbody rb;
   

    [NonSerialized] public Enemy_StateMachine stateMachine;

    [NonSerialized] public GameObject muzzlePoint;
    #endregion

    #region Stats and Trackers
    [HideInInspector] public bool bulletReady = true;

    #endregion

    [Header ("Components")]
    WaveManager waveManager; //Set by waveManager when the enemy object is instantiated
    
    #endregion

    #region SETUP
    void Awake ()
    {
        EnemyGetComponentReferences();
    }

    protected virtual void EnemyGetComponentReferences ()
    {
        stateMachine = GetComponent<Enemy_StateMachine> ();
        rb = GetComponent<Rigidbody> ();

        muzzlePoint = transform.Find ("Body/MuzzlePoint").gameObject;
        Debug.Log (muzzlePoint.name);
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

    public void takeDamage ()
    {
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
