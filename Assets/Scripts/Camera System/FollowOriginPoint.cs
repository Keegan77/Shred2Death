using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FollowOriginPoint : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    private void Update()
    {
        transform.position = originPoint.position;
    }
}
