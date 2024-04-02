using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Upon entering a state, perform a single attack, returning to the previous
/// state afterwards
/// </summary>
public class ES_Attack : Enemy_State, iAttack
{
    public GameObject muzzlePoints { get; set; }
    public Enemy_BulletPattern bulletInfo { get; set; }

    #region StateMachine
    public override void Enter ()
    {
        base.Enter ();
    }

    public override void Exit ()
    {
        base.Exit ();
    }
    #endregion

    public IEnumerator PlayShot ()
    {
        yield return null;
    }
}
