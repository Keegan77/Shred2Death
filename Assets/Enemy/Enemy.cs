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

    [Header ("Bullet")]
    [SerializeField] GameObject bulletType;

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


    public void spawnBullet()
    {
        Rigidbody prb = playerObject.GetComponent<Rigidbody> ();

        GameObject eb = Instantiate (bulletType, transform.position, Quaternion.identity);

        Vector3 solvedPosition = eb.GetComponent<Enemy_Bullet>().LeadShot(playerObject, eb.gameObject);

        eb.transform.LookAt (solvedPosition);
        eb.SetActive (true);


        Debug.DrawLine(playerObject.transform.position, solvedPosition);
        Debug.DrawLine(playerObject.transform.position + new Vector3 (0, 1, 0), ((solvedPosition - playerObject.transform.position).normalized) * playerObject.GetComponent<Rigidbody>().velocity.magnitude + new Vector3 (0, 1, 0) + playerObject.transform.position);
        Debug.DrawLine(playerObject.transform.position + new Vector3 (0, 2, 0), ((solvedPosition - playerObject.transform.position).normalized) * playerObject.GetComponent<Rigidbody>().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + playerObject.transform.position);

        Debug.DrawLine(eb.transform.position, solvedPosition);
        Debug.DrawLine (eb.transform.position + new Vector3 (0, 1, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude + new Vector3 (0, 1, 0) + eb.transform.position);
        Debug.DrawLine (eb.transform.position + new Vector3 (0, 2, 0), eb.transform.forward * eb.GetComponent<Rigidbody> ().velocity.magnitude * 5 + new Vector3 (0, 2, 0) + eb.transform.position);

        Debug.Log (eb.transform.forward);

        

        Debug.Break ();
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
