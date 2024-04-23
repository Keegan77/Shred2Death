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
    IEnumerator playAttack ()
    {
        e.animator.CrossFade ("PREDIVE", 0.2f);
        yield return MoveAnimation (transform.TransformPoint (0, 1, -1), 0.5f, ESF_MoveAnimationType.SMOOTHDAMP);
        

        e.animator.CrossFade ("DIVE", 0.2f);
        yield return MoveAnimation (Enemy.playerReference.aimTarget.transform.position, 15, ESF_MoveAnimationType.MOVETOWARDS);

        e.animator.CrossFade ("DIVEBONK", 0.2f); yield return new WaitForEndOfFrame ();
        yield return MoveAnimation (transform.TransformPoint (0, 0, -5), 0.4f, ESF_MoveAnimationType.SPHERICAL);

        yield return new WaitUntil (() => e.animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 1);
        e.stateMachine.transitionState (exitState);
    }
}
