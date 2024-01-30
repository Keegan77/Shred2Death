using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool trickBeingPerformed;

    private void OnEnable()
    {
        ActionEvents.OnTrickPerformed += StartAnimation;
    }

    private void OnDisable()
    {
        ActionEvents.OnTrickPerformed -= StartAnimation;
    }

    private void StartAnimation(Trick trick)
    {
        animator.SetTrigger(trick.animTriggerName);
        StartCoroutine(TrickAnimationSequence(trick));
    }
    
    private IEnumerator TrickAnimationSequence(Trick trick)
    {
        //assumes the trigger was set the frame before this coroutine was started
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        trickBeingPerformed = true;
        yield return new WaitForSeconds(currentClipInfo.Length);
        
    }
    
}
