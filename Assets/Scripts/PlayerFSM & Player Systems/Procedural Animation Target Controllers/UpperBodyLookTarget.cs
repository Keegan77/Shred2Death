using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBodyLookTarget : MonoBehaviour
{
    [SerializeField] private Transform playerForwardPoint;
    [SerializeField] private Transform cameraForwardPoint;
    [SerializeField] private Transform leftLookPoint, rightLookPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private PlayerBase player;
    [SerializeField] private float targetMoveSpeed;
    private Transform currentTarget;
    private Transform currentLookPoint;

    private float forwardDot, rightDot;
    
    private float PlayerCameraDotProduct => Vector3.Dot(playerForwardPoint.forward, cameraForwardPoint.forward);

    private void Start()
    {
        currentTarget = cameraForwardPoint;
        currentLookPoint = new GameObject().transform;
    }

    private void Update()
    {
        forwardDot = Vector3.Dot(player.transform.forward, cam.forward);
        
        
        //transform.position = currentTarget.position;
        LerpToTargetPosition();
        
        // if slope orientation looking upward or downward
        if (Mathf.Abs(player.GetOrientationWithDownward() - 90) > 30 && player.stateMachine.currentState == player.skatingState) 
        {
            currentTarget = playerForwardPoint; // player looks forward
        }
        else if (player.stateMachine.currentState == player.halfPipeState)
        {
            currentTarget = playerForwardPoint; //player looks towards crosshair
        }
        else
        {
            currentTarget = cameraForwardPoint; // player looks at camera
            if (forwardDot < 0) // if we are not looking forward
            {
                rightDot = Vector3.Dot(player.transform.right, cam.forward);
                if (rightDot > 0) // if we are looking to the right
                {
                    currentLookPoint.position = rightLookPoint.position;
                    currentTarget = currentLookPoint;
                    currentTarget.position = new Vector3(currentTarget.position.x, 
                        cameraForwardPoint.position.y, currentTarget.position.z);
                }
                else
                {
                    currentLookPoint.position = leftLookPoint.position;
                    currentTarget = currentLookPoint;
                    currentTarget.position = new Vector3(currentTarget.position.x, 
                        cameraForwardPoint.position.y, currentTarget.position.z);
                }
            }
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
