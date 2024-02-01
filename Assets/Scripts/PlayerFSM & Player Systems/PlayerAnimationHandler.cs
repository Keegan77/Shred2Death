using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the execution of tricks and animations
/// </summary>
public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerBase player;
    private bool trickBeingPerformed;

    private void OnEnable()
    {
        ActionEvents.OnTrickRequested += TryTrickAnimation;
    }

    private void OnDisable()
    {
        ActionEvents.OnTrickRequested -= TryTrickAnimation;
    }

    private void TryTrickAnimation(Trick trick)
    {
        if (!trickBeingPerformed)
        {
            animator.SetTrigger(trick.animTriggerName);
            StartCoroutine(TrickAnimationSequence(trick));
        }
    }
    
    private IEnumerator TrickAnimationSequence(Trick trick)
    {
        //assumes the trigger was set the frame before this coroutine was started
        yield return null; // waits a frame so the clip info is properly updated with the new clip
        
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0); // 0 refers to the base animation layer
        trickBeingPerformed = true;
        
        Debug.Log("Trick Animation Started");
        ActionEvents.OnTrickPerformed?.Invoke(trick);
        
        if (trick.customMethod != null) trick.customMethod.Invoke(player);
        
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        Debug.Log("Trick Animation Complete");
        
        trickBeingPerformed = false;
        
        ActionEvents.OnTrickCompletion?.Invoke(trick);
        
    }
    
    public bool TrickIsBeingPerformed()
    {
        return trickBeingPerformed;
    }
    
}
