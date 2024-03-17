using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkateboardSlopePositioner : MonoBehaviour
{
    [SerializeField] private PlayerBase player;
    [SerializeField] private float YOffset, ZOffset;
    private Vector3 startingPos;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float rayDistance;

    //[SerializeField] private float raycastZOffsetFromOrigin;

    private void Awake()
    {
        startingPos = transform.localPosition;
    }
    
    private void FixedUpdate()
    {
        if (Physics.Raycast(raycastOrigin.position, -raycastOrigin.up, out RaycastHit hit, rayDistance))
        {
            Vector3 localOffset = transform.TransformDirection(new Vector3(0, YOffset, ZOffset));
            transform.position = hit.point + localOffset;
        }
        else
        {
            transform.localPosition = startingPos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position - raycastOrigin.up * rayDistance);
    }
}
