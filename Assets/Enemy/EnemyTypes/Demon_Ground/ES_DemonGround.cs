using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// States and functionality regarding Grounded Demons' Behavior.
/// Scripts that extend this are abbreviated ES_Ground or ESDG_
/// </summary>
public class ES_DemonGround : Enemy_State
{
    protected E_Demon_Ground eGround;

    #region STATEMACHINE
    private void Awake ()
    {
        eGround = transform.parent.GetComponent<E_Demon_Ground>();
    }

    public override void Enter ()
    {
        base.Enter ();
        eGround.animator.Play (animationEnter);
    }
    #endregion

    #region NAVIGATION
    public void EndPath ()
    {
        eGround.agentPath.ClearCorners ();
    }
    #endregion
}
