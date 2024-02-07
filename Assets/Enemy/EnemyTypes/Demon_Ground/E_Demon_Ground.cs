using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class E_Demon_Ground : Enemy
{

    [NonSerialized] public NavMeshAgent agent; //NavMeshAgent refuses to load in time and now I have to serialize it. Hate.
    [NonSerialized] public Enemy_NavManager agentManger;
    [NonSerialized] public NavMeshPath agentPath;

    //agentSettings will be set by the navmesh present in the level.
    [NonSerialized] public static NavMeshBuildSettings[] agentSettings;
    [NonSerialized] public int agentIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        //rb.isKinematic = false;
        //rb.useGravity = true;

        //Debug.Log ("Checking agent settings");
        //Debug.Log (agent.agentTypeID);
        if (agentSettings != null)
        {
            for (int i = 0; i < agentSettings.Length; i++)
            {
                //Debug.Log (agentSettings[i].agentTypeID);
                if (agentSettings[i].agentTypeID == agent.agentTypeID)
                {
                    //Debug.Log ("Agent Match found");
                    agentIndex = i;

                    //Debug.Log (agentSettings[agentIndex].agentClimb);
                    break;
                }
            }
        }
        else
        {
            Debug.LogError ("No Agent Settings");
        }
    }

    private void Awake ()
    {
        EnemyGetComponentReferences ();
    }

    protected override void EnemyGetComponentReferences ()
    {
        base.EnemyGetComponentReferences ();
        agentManger = GetComponent<Enemy_NavManager> ();
        agent = GetComponent<NavMeshAgent> ();
        agentPath = new NavMeshPath ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
