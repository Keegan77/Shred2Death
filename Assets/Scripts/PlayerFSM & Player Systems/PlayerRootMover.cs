using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The purpose of this class is for when you want to rotate shreddy's
/// player model while having 0 effect on the player's movement.
/// Example use case is the shreddy christ ability.
/// </summary>
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
