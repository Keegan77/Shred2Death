using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBodyLookTarget : MonoBehaviour
{
    [SerializeField] private Transform playerForwardPoint;
    [SerializeField] private Transform cameraForwardPoint;
    [SerializeField] private PlayerBase player;
    [SerializeField] private float targetMoveSpeed;
    private Transform currentTarget;
    
    private float PlayerCameraDotProduct => Vector3.Dot(playerForwardPoint.forward, cameraForwardPoint.forward);

    private void Start()
    {
        currentTarget = cameraForwardPoint;
    }

    private void Update()
    {
        //transform.position = currentTarget.position;
        LerpToTargetPosition();
        
        // if slope orientation looking upward or downward
        if (Mathf.Abs(player.GetOrientationWithDownward() - 90) > 30) 
        {
            currentTarget = playerForwardPoint; // player looks forward
        }
        else
        {
            currentTarget = cameraForwardPoint; //player looks towards crosshair
        }
        
    }

    private void LerpToTargetPosition()
    {
        transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * targetMoveSpeed);
    }

    private void SwitchTargetPoint()
    {
        currentTarget = currentTarget == cameraForwardPoint ? playerForwardPoint : cameraForwardPoint;
    }
}
