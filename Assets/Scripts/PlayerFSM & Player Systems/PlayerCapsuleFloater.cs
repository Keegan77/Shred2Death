using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerCapsuleFloater : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float rayLength;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    [SerializeField] private Transform raycastOrigin;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 downDir = -transform.up;
        
        
        if (Physics.Raycast(raycastOrigin.position, downDir, out hit, rayLength))
        {
            Vector3 vel = rb.velocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down); // downward direction relative to the player's current rotation
            
            float rayDirVel = Vector3.Dot(rayDir, vel);
            
            float x = hit.distance - rideHeight;
            
            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamper);
            
            rb.AddForce(rayDir * springForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastOrigin.position, -transform.up * rayLength);
    }
}
