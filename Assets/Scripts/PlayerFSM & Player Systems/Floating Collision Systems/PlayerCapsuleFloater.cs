using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCapsuleFloater : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] private float rayLength;
    private PlayerBase player;
    public float standingRideHeight;
    public float crouchingRideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    [SerializeField] private Transform raycastOrigin;
    private float currentRideHeight;


    private void Start()
    {
        currentRideHeight = standingRideHeight;
        player = GetComponent<PlayerBase>();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 downDir = -transform.up;

        if (player.stateMachine.currentState == player.airborneState) return;
        if (player.stateMachine.currentState == player.deathState) return;
        
        //TODO: This method is the reason for the inconsistent jump height bug, needs tweaking
        if (Physics.Raycast(raycastOrigin.position, downDir, out hit, rayLength))
        {
            Vector3 vel = rb.velocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down); // downward direction relative to the player's current rotation
            
            float rayDirVel = Vector3.Dot(rayDir, vel);
            //dot product is a measure of how much of vel is in the direction of rayDir. If the player is moving
            //locally downward, rayDirVel will be a large positive number. If it's moving locally upwards,
            //it will be a large negative number.
            
            float x = hit.distance - currentRideHeight; //this mag determines how much spring force strength we need to apply
            
            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamper);
            
            rb.AddForce(rayDir * springForce);
        }
    }

    public void SetRideHeight(float newRideHeight)
    {
        currentRideHeight = newRideHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(raycastOrigin.position, transform.TransformDirection(Vector3.down) * rayLength);
    }
}
