using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RBTriggerForwarder : MonoBehaviour
{
    [SerializeField] private PlayerBase childScript;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        childScript.PlayerTriggerEnter(other);
    }

    private void Update()
    {
        rb.centerOfMass = childScript.transform.up * 2;
    }

    private void OnTriggerStay(Collider other)
    {
        childScript.PlayerTriggerStay(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        childScript.PlayerTriggerExit(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        childScript.PlayerCollisionEnter(other);
    }*/
    
}
