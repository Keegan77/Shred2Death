using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// States and functionality regarding Grounded Demons' Behavior.
/// Scripts that extend this are abbreviated ES_Ground or ESDG_
/// </summary>
public abstract class ES_DemonGround : Enemy_State
{
    protected E_Demon_Ground eg;

    #region STATEMACHINE
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy> ();
        eg = transform.parent.GetComponent<E_Demon_Ground>();
    }

    public override void Enter ()
    {
        base.Enter ();
        eg.animator.Play (animationEnter);

        eg.agent.isStopped = false;
        eg.agent.updatePosition = true;
        eg.agent.Warp (transform.position);
    }

    public override void Exit ()
    {
        base.Exit ();
        eg.agent.ResetPath ();
        eg.agent.isStopped = true;
    }
    #endregion

    #region NAVIGATION
    public void EndPath ()
    {
        eg.agentPath.ClearCorners ();
    }

    /// <summary>
    /// Handles the movement navigation of the 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected IEnumerator MoveToPoint (Vector3 p, string animName, bool overwriteAnim = true)
    {
        yield return null;
        eg.agent.CalculatePath (p, eg.agentPath);
        
        if(eg.agentPath.status != UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            eg.agent.SetDestination (p);
            eg.animator.Play (animName);
        }
        else
        {
            Debug.LogWarning ($"{name}: Could not find path to point, ending MTP", this);

            OnDestinationFailed ();
            yield break;
        }
        
        Vector3 distanceToDestination = eg.agent.destination - transform.position;
        while (distanceToDestination.magnitude > eg.agent.stoppingDistance)
        {
            distanceToDestination = eg.agent.destination - transform.position;
            yield return null;
        }

        OnDestinationReached ();
    }

    protected abstract void OnDestinationReached ();
    protected abstract void OnDestinationFailed ();
    #endregion
}
