using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreOriginRotation : MonoBehaviour
{
    Quaternion lastParentRotation;

    private void Start()
    {
        lastParentRotation = transform.parent.localRotation;
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Inverse(lastParentRotation) * lastParentRotation * transform.parent.localRotation;
        lastParentRotation = transform.parent.localRotation;
    }
}
