using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowNavAgent : MonoBehaviour
{
    GameObject agentObject;
    private void Start ()
    {
        agentObject = transform.Find ("AgentObject").gameObject;
    }

    private void FixedUpdate ()
    {
        transform.position = agentObject.transform.position;
        agentObject.transform.position = transform.position;

        transform.rotation = agentObject.transform.rotation;
        agentObject.transform.rotation = transform.rotation;
    }
}
