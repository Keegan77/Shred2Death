using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetProperties : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    private void LateUpdate()
    {
        transform.position = originPoint.position;
        transform.rotation = originPoint.rotation;
    }

}

