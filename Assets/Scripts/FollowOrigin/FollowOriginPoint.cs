using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FollowOriginPoint : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    [SerializeField] private Transform airOriginPoint;
    [SerializeField] private PlayerBase player;
    [SerializeField] private float smoothTime;
    [SerializeField] private bool dampY;
    private Vector3 velocity = Vector3.zero;
    float t;
    private void LateUpdate()
    {
        if (dampY)
        {
            float newY = Mathf.SmoothDamp(transform.position.y, originPoint.position.y, ref velocity.y, smoothTime);
            transform.position = new Vector3(originPoint.position.x, newY, originPoint.position.z);
        }
        else
        {
            if (player.stateMachine.currentState != player.skatingState)
            {
                transform.position = airOriginPoint.position;
            }
            else
            {
                transform.position = originPoint.position;
            }
        }
        
    }
}
