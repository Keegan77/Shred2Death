using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigWeightController : MonoBehaviour
{
    public Rig armRig, legRig, headAndChestRig;
    public Coroutine currentRigWeightRequest;


    private void OnEnable()
    {
        ActionEvents.OnTrickCompletion += HandleOnTrickCompletion;
    }

    private void OnDisable()
    {
        ActionEvents.OnTrickCompletion -= HandleOnTrickCompletion;
    }
    
    private void HandleOnTrickCompletion(Trick trick)
    {
        currentRigWeightRequest = null;
    }
    
    private IEnumerator LerpWeightToValue(Rig rig, float targetWeight, float lerpTime)
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
    
    public void SetWeightToValueOverTime(Rig rig, float targetWeight, float lerpTime, bool useThisAsRequest = true)
    {
        if (currentRigWeightRequest != null)
        {
            StopCoroutine(currentRigWeightRequest);
        }

        if (useThisAsRequest)
        {
            currentRigWeightRequest = StartCoroutine(LerpWeightToValue(rig, targetWeight, lerpTime));
        }
        else
        {
            StartCoroutine(LerpWeightToValue(rig, targetWeight, lerpTime));
        }
        
    }
    
    public void SetWeightToValue(Rig rig, float targetWeight)
    {
        if (currentRigWeightRequest != null)
        {
            StopCoroutine(currentRigWeightRequest);
        }
        rig.weight = targetWeight;
    }


    public IEnumerator SetWeightToValueOverSeconds(Rig rig, float secondsSpentAtWeight, float blendTime)
    {
        SetWeightToValueOverTime(rig, 0, blendTime, useThisAsRequest:false);
        yield return new WaitForSeconds(secondsSpentAtWeight);

        if (currentRigWeightRequest == null) SetWeightToValueOverTime(rig, 1, blendTime);
    }
    

}
