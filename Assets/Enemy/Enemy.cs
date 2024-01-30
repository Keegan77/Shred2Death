using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy_StateMachine))]
public class Enemy : MonoBehaviour
{

    #region PAREMETERS

    [Header("Stats")]
    public int health;
    public float movementSpeed;

    #endregion

    #region SCRIPT VARIABLES
    [Header("Components")]
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public NavMeshAgent agent; //NavMeshAgent refuses to load in time and now I have to serialize it. Hate.
    [NonSerialized] public Enemy_NavManager agentManger;
    [NonSerialized] public NavMeshPath agentPath;

    [NonSerialized] public Enemy_StateMachine stateMachine;

    WaveManager waveManager; //Set by waveManager when the enemy object is instantiated
    
    #endregion

    #region SETUP
    void Awake ()
    {
        stateMachine = GetComponent<Enemy_StateMachine> ();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent> ();
        agentManger = GetComponent<Enemy_NavManager> ();

        agentPath = new NavMeshPath ();
    }

    private void Start ()
    {
        //rb.isKinematic = false;
        //rb.useGravity = true;
    }

    public void setManager (WaveManager w)
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
