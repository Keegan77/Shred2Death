using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Upon entering a state, perform a single attack, returning to the previous
/// state afterwards.
/// </summary>
public class ES_Attack : Enemy_State, iAttack
{
    [SerializeField] private GameObject _muzzlePoint;
    [SerializeField] private Enemy_BulletPattern _bulletInfo;
    public GameObject muzzlePoint { get { return _muzzlePoint; } set { muzzlePoint = value; } }
    public Enemy_BulletPattern bulletInfo { get { return _bulletInfo; } set { _bulletInfo = value; } }

    #region StateMachine
    public override void Enter ()
    {
        base.Enter ();
        StartCoroutine (PlayShot ());
        
    }

    public override void Exit ()
    {
        base.Exit ();
        StopAllCoroutines ();
        bulletInfo.CancelShot ();

    }

    public override void machineUpdate ()
    {
        Vector3 lookTarget = new Vector3 (
            Enemy.playerReference.transform.position.x,
            transform.position.y,
            Enemy.playerReference.transform.position.z
            );
        e.transform.rotation = Quaternion.LookRotation (lookTarget, Vector3.up);
    }
    #endregion

    public IEnumerator PlayShot ()
    {
        if (bulletInfo.bulletReady)
        {
            bulletInfo.reserveTokens ();
            e.animator.Play (bulletInfo.attackAnimation);
            yield return bulletInfo.PlayShot (Enemy.playerReference.gameObject, muzzlePoint);
        }

        e.stateMachine.transitionState (e.stateMachine.statePrevious);

        bulletInfo.returnTokens ();

    }
}
