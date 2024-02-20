using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOriginRotation : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    private void LateUpdate()
    {
        transform.rotation = originPoint.rotation;
    }

}
