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
    public bool trickBeingPerformed;
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
        animator.CrossFade(triggerName, 0.1f);
    }

    private Coroutine trickCoroutine;
    private void TryTrickAnimation(Trick trick)
    {
        if (!trickBeingPerformed)
        {
            /*if (interruptState)
                if (trickCoroutine != null) StopCoroutine(trickCoroutine);*/
            
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
        if (trick.skipAnim)
        {
            if (trick.customMethod != null) trick.customMethod.Invoke(player);
            ActionEvents.OnTrickPerformed?.Invoke(trick);
            ActionEvents.OnTrickCompletion?.Invoke(trick);
            // stop coroutine
            yield break;
        }
        
        animator.CrossFade(trick.animTriggerName, 0.1f);
        trickBeingPerformed = true;
        
        ActionEvents.OnTrickPerformed?.Invoke(trick);
        
        if (trick.customMethod != null) trick.customMethod.Invoke(player);
        player.proceduralRigController.SetWeightToValueOverTime(player.proceduralRigController.legRig, 
                                                     0, 
                                                                player.playerData.animBlendTime);
        
        //yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        yield return new WaitForSeconds(trick.animTime);
        
        trickBeingPerformed = false;
        
        ActionEvents.OnTrickCompletion?.Invoke(trick);
        player.proceduralRigController.SetWeightToValueOverTime(player.proceduralRigController.legRig, 
            1, 
            player.playerData.animBlendTime);
        
    }
    
    public bool TrickIsBeingPerformed()
    {
        return trickBeingPerformed;
    }
    
}
