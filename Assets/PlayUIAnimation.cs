using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUIAnimation : MonoBehaviour
{
    Animator animator;
    [SerializeField] string activeAnimationName;
    [SerializeField] string inactiveAnimationName;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ActionEvents.OnAbilityStateSwitch += PlayAnimationOnBoost;
    }

    private void OnDisable()
    {
        ActionEvents.OnAbilityStateSwitch -= PlayAnimationOnBoost;
    }

    private void PlayAnimationOnBoost(Type ability)
    {
        if (ability == typeof(BoostAbilityState))
        {
            animator.Play(activeAnimationName);
        }
        else
        {
            animator.Play(inactiveAnimationName);
        }
    }
}
