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
    private bool interruptState;

    private void OnEnable()
    {
        ActionEvents.OnTrickRequested += TryTrickAnimation;
        ActionEvents.OnPlayBehaviourAnimation += PlayBehaviourAnimation;
    }

    private void OnDisable()
    {
        ActionEvents.OnTrickRequested -= TryTrickAnimation;
        ActionEvents.OnPlayBehaviourAnimation -= PlayBehaviourAnimation;
    }

    private void PlayBehaviourAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    private Coroutine trickCoroutine;
    private void TryTrickAnimation(Trick trick)
    {
        if (!trickBeingPerformed)
        {
            /*if (interruptState)
                if (trickCoroutine != null) StopCoroutine(trickCoroutine);*/
            animator.SetTrigger(trick.animTriggerName);
            trickCoroutine = StartCoroutine(TrickAnimationSequence(trick));
        }
    }
    
    private IEnumerator TrickAnimationSequence(Trick trick)
    {
        //assumes the trigger was set the frame before this coroutine was started
        /*yield return null; // waits a frame so the clip info is properly updated with the new clip
        if (trick.canBeInterrupted)
        {
            interruptState = true;
        }
        else
        {
            interruptState = false;
        }*/

        yield return new WaitForSeconds(.1f);
        
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0); // 0 refers to the base animation layer
        trickBeingPerformed = true;
        Debug.Log(currentClipInfo[0].clip.name);
        
        ActionEvents.OnTrickPerformed?.Invoke(trick);
        
        if (trick.customMethod != null) trick.customMethod.Invoke(player);
        player.proceduralRigController.SetWeightToValue(player.proceduralRigController.legRig, 0);
        
        //yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        yield return new WaitUntil(() => currentClipInfo[0].clip.name == "Shreddy_Skatepose");
        
        trickBeingPerformed = false;
        
        ActionEvents.OnTrickCompletion?.Invoke(trick);
        player.proceduralRigController.SetWeightToValue(player.proceduralRigController.legRig, 1);
        
    }
    
    public bool TrickIsBeingPerformed()
    {
        return trickBeingPerformed;
    }
    
}
