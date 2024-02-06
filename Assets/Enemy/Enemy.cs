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

    #region PAREMETERS

    [Header("Stats")]
    public int health;
    public float movementSpeed;

    #endregion

    #region SCRIPT VARIABLES

    public static GameObject playerObject;

    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public NavMeshAgent agent; //NavMeshAgent refuses to load in time and now I have to serialize it. Hate.
    [NonSerialized] public Enemy_NavManager agentManger;
    [NonSerialized] public NavMeshPath agentPath;

    //agentSettings will be set by the navmesh present in the level.
    [NonSerialized] public static NavMeshBuildSettings[] agentSettings;
    [NonSerialized] public int agentIndex = 0;

    [NonSerialized] public Enemy_StateMachine stateMachine;

    [NonSerialized] public GameObject muzzlePoint;

    [Header ("Components")]
    WaveManager waveManager; //Set by waveManager when the enemy object is instantiated
    
    #endregion

    #region SETUP
    void Awake ()
    {
        stateMachine = GetComponent<Enemy_StateMachine> ();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent> ();
        agentManger = GetComponent<Enemy_NavManager> ();

        muzzlePoint = transform.Find ("Body/MuzzlePoint").gameObject;

        agentPath = new NavMeshPath ();
    }

    //After it's spawned, the static variable for agentSettings should exist.
    private void Start ()
    {
        //rb.isKinematic = false;
        //rb.useGravity = true;

        Debug.Log (agent.agentTypeID);
        if(agentSettings != null)
        {
            for (int i = 0; i < agentSettings.Length; i++)
            {
                Debug.Log (agentSettings[i].agentTypeID);
                if (agentSettings[i].agentTypeID == agent.agentTypeID)
                {
                    Debug.Log ("Agent Match found");
                    agentIndex = i;

                    Debug.Log (agentSettings[agentIndex].agentClimb);
                    break;
                }
            }
        }
        else
        {
            Debug.LogError ("No Agent Settings");
        }
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
