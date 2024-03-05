using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetProperties : MonoBehaviour
{ 
    private Transform originPoint;
    private bool movingArms;
    private void LateUpdate()
    {
        if (movingArms) return;
        transform.position = originPoint.position;
        transform.rotation = originPoint.rotation;
    }
    
    public void SetOriginPoint(Transform origin)
    {
        movingArms = true;
        originPoint = origin;
        StartCoroutine(LerpToNewOriginPoint(origin, .2f));
    }
    
    private IEnumerator LerpToNewOriginPoint(Transform newOrigin, float lerpTime)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            
        while (timeElapsed < lerpTime)
        {
            timeElapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, newOrigin.position, curve.Evaluate(timeElapsed / lerpTime));
            transform.rotation = Quaternion.Lerp(startRot, newOrigin.rotation, curve.Evaluate(timeElapsed / lerpTime));
            yield return null;
        }
        movingArms = false;
    }

}

