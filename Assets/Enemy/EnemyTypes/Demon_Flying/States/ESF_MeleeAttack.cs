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

    IEnumerator playAttack ()
    {
        yield return MoveAnimation (transform.TransformPoint (0, 1, -1), 0.5f, ESF_MoveAnimationType.SMOOTHDAMP);
        yield return new WaitForSeconds (0.5f);
        yield return MoveAnimation (Enemy.playerReference.aimTarget.transform.position, 30, ESF_MoveAnimationType.MOVETOWARDS);
        e.stateMachine.transitionState (exitState);
    }
}
