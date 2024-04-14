using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRootMover : MonoBehaviour
{
    public IEnumerator RotateBackToZero()
    {
        float t = 0;
        while (t < 2)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, t);
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
    }
}
