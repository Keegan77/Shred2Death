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
    private Queue<string> behaviourAnimationQueue = new Queue<string>();
    private bool trickBeingPerformed;

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
        behaviourAnimationQueue.Enqueue(triggerName);
        if (behaviourAnimationQueue.Count == 1)
        {
            animator.SetTrigger(triggerName);
            StartCoroutine(BehaviourStateAnimationSequencer(triggerName));
        }
    }
    
    public void DebugQueueElements()
    {
        Debug.Log("Current queue elements:");
        foreach (string triggerName in behaviourAnimationQueue)
        {
            Debug.Log(triggerName);
        }
    }
    
    
    private IEnumerator BehaviourStateAnimationSequencer(string triggerName)
    {
        DebugQueueElements();
        yield return null; // waits a frame so the clip info is properly updated with the new clip
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0); // 0 refers to the base animation layer
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        PlayNextAnimation();
    }
    
    private void PlayNextAnimation()
    {
        if (behaviourAnimationQueue.Count > 0)
        {
            behaviourAnimationQueue.Dequeue();
            if (behaviourAnimationQueue.Count > 0)
            {
                string nextTriggerName = behaviourAnimationQueue.Peek();
                animator.SetTrigger(nextTriggerName);
                StartCoroutine(BehaviourStateAnimationSequencer(nextTriggerName));
            }
        }
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
        
        ActionEvents.OnTrickPerformed?.Invoke(trick);
        
        if (trick.customMethod != null) trick.customMethod.Invoke(player);
        
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        
        trickBeingPerformed = false;
        
        ActionEvents.OnTrickCompletion?.Invoke(trick);
        
    }
    
    public bool TrickIsBeingPerformed()
    {
        return trickBeingPerformed;
    }
    
}
