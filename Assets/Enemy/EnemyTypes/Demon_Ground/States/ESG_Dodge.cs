using System.Collections;
using UnityEngine;


/// <summary>
/// When the associated sensor trips this state, the enemy will dodge to the left or right of the player depending on which is more convenient for it at the time.
/// It will move in that direction by using a raycast until it hits 
/// </summary>
public class ESG_Dodge : ES_DemonGround
{
    Rigidbody eBody;

    #region Movement Paramaters

    [Tooltip("Adjust the curve to fine tune the pacing of the dodge")]
    public AnimationCurve curvePosition;

    [Tooltip("How far does the dodge go?")]
    public float dodgeDistance;

    [Tooltip("How long does it take to start and finish the dodge?")]
    public float dodgeTime;

    #endregion
    private void Start ()
    {
        eBody = eg.GetComponent<Rigidbody>();
        e.stateMachine.transitionState (this);
    }

    #region State Machine
    public override void Enter ()
    {
        base.Enter ();
        eg.agent.ResetPath ();
        eg.agent.isStopped = true;
        eg.agent.updatePosition = false;
    }

    public override void Exit ()
    {
        base.Exit ();
        eg.agent.Warp (transform.position);
        eg.agent.updatePosition = true;
    }


    /// <summary>
    /// 
    /// </summary>
    public override void machinePhysics ()
    {
        
    }

    #endregion

    #region Navigation
    protected override void OnDestinationFailed ()
    {
        throw new System.NotImplementedException ();
    }

    protected override void OnDestinationReached ()
    {
        throw new System.NotImplementedException ();
    }

    #endregion
}