using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enemy Animations sometimes have events on them, flagging certain behaviors.
/// This script picks up these events and works with them as Unity Events
/// </summary>
public class Enemy_AnimEventHandler : MonoBehaviour
{
    public UnityEvent event_OnAnimationFinished;
    public UnityEvent event_OnBullet;

    public void OnAnimationFinished ()
    {
        event_OnAnimationFinished.Invoke ();
    }
    public void OnBullet ()
    {
        event_OnBullet.Invoke ();
    }

}
