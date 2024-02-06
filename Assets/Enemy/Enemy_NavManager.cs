using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_NavManager : MonoBehaviour
{
    #region PARAMETERS
    [Header ("Movement")]
    public float time_WaitForJump = 0;

    [Header ("AgentSettings")]
    public bool autoTraverseOffMeshLink = false;
    public bool updatePosition = true;
    public bool updateRotate = true;
    public bool updateUpAxis = true;
    #endregion

    #region SCRIPT VARIABLES
    NavMeshAgent agent;

    #endregion

    private void Awake ()
    {
        agent = transform.Find("AgentObject").GetComponent<NavMeshAgent>();

        agent.autoTraverseOffMeshLink = autoTraverseOffMeshLink;
        agent.updatePosition = updatePosition;
        agent.updateRotation = updateRotate;
        agent.updateUpAxis = updateUpAxis;
    }

    private void FixedUpdate ()
    {
        if (agent.isOnOffMeshLink)
        {
            //Debug.Log ("Agent is on the link");
            NavMovement_Jump ();
        }
    }

    bool isJumping = false;
    public void NavMovement_Jump ()
    {
        if(!isJumping)
        {
            StartCoroutine(WaitForJump());
        }
    }
    
    //When enemies decide to jump I want them to wait for a second before doing so
    IEnumerator WaitForJump ()
    {
        isJumping = true;
        agent.isStopped = true;

        yield return new WaitForSeconds (time_WaitForJump);
        agent.isStopped = false;
        agent.autoTraverseOffMeshLink = true;

        while (agent.isOnOffMeshLink)
        {
            yield return null;
        }
        
        //Debug.Log ("Thingy finished jumping");
        isJumping = false;

    }
}
