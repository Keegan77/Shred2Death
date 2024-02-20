using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsInRangeOf(this float value, float min, float max)
    {
        return value >= min && value <= max;
    }
    
    public static Vector3 GetLocalVelocity(this Rigidbody rb)
    {
        return rb.transform.InverseTransformDirection(rb.velocity);
    }
    
    public static void SetLocalAxisVelocity(this Rigidbody rb, Vector3 axis, float velocity)
    {
        Vector3 localVelocity = rb.transform.InverseTransformDirection(rb.velocity);

        localVelocity = new Vector3(
            axis.x != 0 ? velocity * Mathf.Sign(axis.x) : localVelocity.x,
            axis.y != 0 ? velocity * Mathf.Sign(axis.y) : localVelocity.y,
            axis.z != 0 ? velocity * Mathf.Sign(axis.z) : localVelocity.z
        );

        rb.velocity = rb.transform.TransformDirection(localVelocity);
    }
}
