using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
    private float dodgeTimerPrevious = 0;
    private float dodgePositionPrevious = 0;

    [Tooltip("How far does the dodge go?")]
    public float dodgeDistance;

    [Tooltip("How long does it take to start and finish the dodge?")]
    public float dodgeTime;

    Vector3 DodgeDirection;

    #endregion
    private void Start ()
    {
        eBody = eg.GetComponent<Rigidbody>();
    }

    #region State Machine
    public override void Enter ()
    {
        base.Enter ();

        //Stop any nav agent interference in movement
        eg.agent.ResetPath ();
        eg.agent.isStopped = true;
        eg.agent.updatePosition = false;

        dodgeTimerPrevious = 0;
        dodgePositionPrevious = 0;

        eBody.isKinematic = false;

        //Decide on which way to dodge
        Rigidbody prb = Enemy.playerReference.GetComponent<Rigidbody>();

            //Compare rotation of player's movement to current rotation of enemy
        Debug.Log (Mathf.Rad2Deg * Mathf.Atan2 (prb.velocity.x, prb.velocity.z));
        Debug.Log (Mathf.Rad2Deg * transform.rotation.y);
        //if ( Mathf.Atan2 (prb.velocity.x, prb.velocity.z));
    }

    public override void Exit ()
    {
        base.Exit ();
        eg.agent.Warp (transform.position);

        eBody.isKinematic = true;
    }


    /// <summary>
    /// Moves the enemy in the specified direction according to parameters
    /// </summary>
    public override void machinePhysics ()
    {
        float movementMagnitude = curvePosition.Evaluate (e.stateMachine.timerCurrentState / dodgeTime) * dodgeDistance;
        eBody.velocity = transform.right * ((movementMagnitude - dodgePositionPrevious) / Time.deltaTime);

        //Debug.Log ($"{movementMagnitude}");
        //Debug.Log ($"<color=red> {movementMagnitude - dodgePositionPrevious} </color>");
        //Debug.Log ($"<color=green> {(movementMagnitude - dodgePositionPrevious) / Time.deltaTime} </color>");

        dodgePositionPrevious = movementMagnitude;
        dodgeTimerPrevious = e.stateMachine.timerCurrentState;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition (transform.position, out hit, 0.5f, eg.agent.areaMask))
        {
            ES_Ragdoll ragdoll = GetComponent<ES_Ragdoll> ();
            ragdoll.entryVelocity = eBody.velocity;
            ragdoll.EnterRagdoll (false);

        }

        if (e.stateMachine.timerCurrentState > dodgeTime)
        {
                e.stateMachine.transitionState (GetComponent<ESG_Empty> ());
        }
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