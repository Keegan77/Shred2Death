using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// States are the behaviors of how an enemy moves around and shoots at the player.
/// 
/// There are a few basic states that derive off of this basic state,
/// which classes outside of the enemy object may reference.
/// </summary>
public class Enemy_State : MonoBehaviour
{
    //public static GameObject playerObject
    protected Enemy e;

    //Is the enemy currently playing an animation?
    protected bool isAnimationPlaying = false;

    [Header ("Debugging")]
    [SerializeField] protected bool stateDebugLogging;

    [Header ("Animation")]
    public string animationEnter = "";
    [SerializeField, Range (0, 1)] float crossFadeTime = 0.5f;
    [SerializeField] bool playOnEnter = false;

    //States will control movement directly.
    //Rigidbody will be set in the start function

    #region STATE MACHINE

    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy> ();
    }

    /// <summary>
    /// After entering the state 
    /// </summary>
    public virtual void Enter ()
    {
        if(stateDebugLogging) Debug.Log ($"<Color=#ffff00>{e.name}</color> (<color=#ffff00>{e.gameObject.GetInstanceID ()})</color>: <Color=#00ff00>{this}</color> Entered");
        if (playOnEnter) e.animator.CrossFade (animationEnter, crossFadeTime);

    }

    public virtual void Exit ()
    {
        StopAllCoroutines ();
        if (stateDebugLogging) Debug.Log ($"<Color=#ffff00>{e.name}</color> (<color=#ffff00>{e.gameObject.GetInstanceID ()})</color>: <Color=#00ff00>{this}</color> Exited");
    }

    public virtual void machineUpdate ()
    {

    }

    public virtual void machinePhysics ()
    {

    }

    public virtual void AIUpdate ()
    {
        //Debug.Log ($"{e.name} ({GetInstanceID()}): Updating AI", this);
    }


    #endregion

    #region EVENT RECEPTORS
    public virtual void onPlayerSensorActivated ()
    {

    }

    public virtual void onPlayerSensorDeactivated ()
    {

    }

    public virtual void OnBullet ()
    {

    }

    public virtual void OnAnimationFinished ()
    {
        isAnimationPlaying = false;
    }

    #endregion

    #region CONTEXT MENU
    [ContextMenu ("EnterState")]
    public void EnterState ()
    {
        if (Application.isPlaying)
            e.stateMachine.transitionState (this);
    }

    [ContextMenu ("PlayAnimation")]
    public void PlayAnimation ()
    {
        if (Application.isPlaying)
            e.animator.Play (animationEnter);
    }
    #endregion

    #region COROUTINES

    #endregion
}
