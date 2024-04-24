using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESF_MeleeAttack : EState_Flying
{

    #region 
    [Header ("Object references")]

    [SerializeField] GameObject meleeSensor;
    [SerializeField] Enemy_State exitState;
    #endregion

    public override void Enter ()
    {
        base.Enter ();
        StartCoroutine (playAttack());
    }

    public override void machineUpdate ()
    {
        base.machineUpdate ();
        Debug.Log (e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime);
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
        e.animator.CrossFade ("PREDIVE", 0.2f);
        yield return MoveAnimation (transform.TransformPoint (0, 1, -1), attackStartup);
        

        e.animator.CrossFade ("DIVE", 0.2f);
        yield return MoveAnimation (Enemy.playerReference.aimTarget.transform.position, attackDive);

        e.animator.CrossFade ("DIVEBONK", 0.2f); yield return new WaitForEndOfFrame ();
        yield return MoveAnimation (transform.TransformPoint (0, 0, -5), attackRebound);

        yield return new WaitUntil (() => e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 1);
        e.stateMachine.transitionState (exitState);
    }
}
