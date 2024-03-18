using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkateboardSlopePositioner : MonoBehaviour
{
    [SerializeField] private PlayerBase player;
    [SerializeField] private float ZOffset;
    [SerializeField] private float verticalYOffset, horizontalYOffset;
    private Vector3 startingPos;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float rayNormalDistance; //1.3
    [SerializeField] private float rayHalfPipeDist;
    private float rayDistance;

    //[SerializeField] private float raycastZOffsetFromOrigin;

    private void Awake()
    {
        startingPos = transform.localPosition;
    }
    
    private void FixedUpdate()
    {
        SetRayDistances();
        //Debug.Log(player.GetOrientationWithDownward() - 90);
        
        float YOffset;
        if (player.stateMachine.currentState != player.halfPipeState)
        {
            float percentageToNinety = Mathf.Abs(player.GetOrientationWithDownward() - 90) / 90;
            YOffset = Mathf.Lerp(verticalYOffset, horizontalYOffset, percentageToNinety); // YOffset needs to be dynamically changed based on the player's orientation
        }
        else
        {
            YOffset = horizontalYOffset;
        }
        
        if (Physics.Raycast(raycastOrigin.position, -raycastOrigin.up, out RaycastHit hit, rayDistance))
        {
            Vector3 localOffset = transform.TransformDirection(new Vector3(0, YOffset, ZOffset));
            transform.position = hit.point + localOffset;
        }
        else
        {
            //transform.localPosition = startingPos;
        }
    }

    private void SetRayDistances()
    {
        if (player.stateMachine.currentState != player.halfPipeState)
        {
            rayDistance = rayNormalDistance;
        }
        else
        {
            rayDistance = rayHalfPipeDist;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position - raycastOrigin.up * rayDistance);
    }
}
