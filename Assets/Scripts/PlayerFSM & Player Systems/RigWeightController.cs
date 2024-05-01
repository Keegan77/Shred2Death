using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigWeightController : MonoBehaviour
{
    public Rig armRig, legRig, headAndChestRig;
    
    public IEnumerator LerpWeightToValue(Rig rig, float targetWeight, float lerpTime)
    {
        float timeElapsed = 0;
        float startWeight = rig.weight;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        while (timeElapsed < lerpTime)
        {
            timeElapsed += Time.deltaTime;
            rig.weight = Mathf.Lerp(startWeight, targetWeight, curve.Evaluate(timeElapsed / lerpTime));
            yield return null;
        }
    }
    
    public void SetWeightToValue(Rig rig, float targetWeight)
    {
        rig.weight = targetWeight;
    }
    
    public IEnumerator SetWeightToValueForSeconds(Rig rig, float targetWeight, float seconds)
    {
        float cachedWeight = rig.weight;
        rig.weight = targetWeight;
        yield return new WaitForSeconds(seconds);
        rig.weight = 1;
    }
}
