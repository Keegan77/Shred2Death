using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESF_MeleeAttack : EState_Flying
{

    #region 
    [Header ("Object references")]

    [SerializeField] GameObject meleeSensor; //Radius sensor that makes the flying enemy bonk
    [SerializeField] Enemy_State exitState;
    #endregion

    public override void Enter ()
    {
        base.Enter ();
        StartCoroutine (playAttack());
    }
    public override void Exit ()
    {
        movementDirection = transform.forward;
        movementAvoidance = transform.forward;

        meleeSensor.SetActive (false);
        e.rb.velocity = transform.forward;
        base.Exit ();
    }

    public override void machineUpdate ()
    {
        base.machineUpdate ();
        //Debug.Log (e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime);
    }

    [Header("Attack Movement")]
    [SerializeField] ESF_MovementOptions attackStartup;
    [SerializeField] ESF_MovementOptions attackDive;
    [SerializeField] ESF_MovementOptions attackRebound;

    /// <summary>
    /// Uses moveanimation to play out the motion of the somersault attack.
    /// 
    /// TODO: Use sensors to conduct bonking better
    /// </summary>
    /// <returns></returns>
    IEnumerator playAttack ()
    {
        Coroutine tracker = StartCoroutine (TrackPlayer ());

        e.animator.CrossFade ("PREDIVE", 0.2f);
        yield return MoveAnimation (transform.TransformPoint (attackStartup.moveTarget), attackStartup);
        yield return new WaitUntil (() => e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1f);

        StopCoroutine (tracker);

        e.animator.CrossFade ("DIVE", 0.2f);
        meleeSensor.SetActive (true);
        yield return MoveAnimation (Enemy.playerReference.aimTarget.transform.position, attackDive);

        meleeSensor.SetActive (false);
        e.animator.CrossFade ("FLAP", 2f);
        yield return new WaitForSeconds (0.5f);
        e.stateMachine.transitionState (exitState);
    }

    public void onAttackCollided ()
    {
        StartCoroutine (playBonk ());
    }

    /// <summary>
    /// Called by onAttackCollided. cancels the attack animation into a recoil movement.
    /// </summary>
    /// <returns></returns>
    IEnumerator playBonk ()
    {
        meleeSensor.SetActive (false);
        StopCoroutine (playAttack ());

        e.animator.CrossFade ("DIVEBONK", 0.2f);
        yield return new WaitForEndOfFrame ();
        yield return MoveAnimation (transform.TransformPoint (attackRebound.moveTarget), attackRebound);
        //yield return new WaitUntil (() => e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 1);
        //yield return new WaitForFixedUpdate ();

        e.stateMachine.transitionState (exitState);
    }
}
