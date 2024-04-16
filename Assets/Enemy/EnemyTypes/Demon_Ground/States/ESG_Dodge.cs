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

    [Header("Dodge Parameters")]

    [Tooltip("Adjust the curve to fine tune the pacing of the dodge")]
    public AnimationCurve curvePosition;
    private float dodgeTimerPrevious = 0;
    private float dodgePositionPrevious = 0;

    [Tooltip("How far does the dodge go?")]
    public float dodgeDistance;

    [Tooltip("How long does it take to start and finish the dodge?")]
    public float dodgeTime;

    Vector3 DodgeDirection = Vector3.right;

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


        dodgeTimerPrevious = 0;
        dodgePositionPrevious = 0;

        eBody.isKinematic = false;

        //Decide on which way to dodge
        Rigidbody prb = Enemy.playerReference.GetComponent<Rigidbody>();

        //Compare rotation of player's movement to current rotation of enemy
        Vector3 epos = transform.position;
        Vector3 ppos = prb.transform.position;
        Vector3 pvel = prb.velocity;

        //Returns the angle left or right of the enemy the player is going
        float anglePlayerEntry = Vector3.SignedAngle (pvel, epos - ppos, Vector3.up);
        float dotEnemyFacingPlayer = Vector3.Dot(transform.forward, pvel.normalized); //returns true if the enemy is facing the player
        //Debug.Log (epos);
        //Debug.Log (ppos);
        //Debug.Log (pvel);

        //Debug.Log (epos - ppos);
        //Debug.Log (anglePlayerEntry);
        //Debug.Log (dotEnemyFacingPlayer);

        //If coming at the player from behind, dodge based on this angle
        if (dotEnemyFacingPlayer > 0)
            DodgeDirection = (anglePlayerEntry > 0) ? transform.right : -transform.right;
        else 
            DodgeDirection = (anglePlayerEntry > 0) ? -transform.right : transform.right;

    }

    public override void Exit ()
    {
        eg.agent.Warp (transform.position);
        //eg.agent.enabled = true;

        base.Exit ();

        eBody.isKinematic = true;
    }


    /// <summary>
    /// Moves the enemy in the specified direction according to parameters
    /// </summary>
    public override void machinePhysics ()
    {
        float movementMagnitude = curvePosition.Evaluate (e.stateMachine.timerCurrentState / dodgeTime) * dodgeDistance;
        eBody.velocity = DodgeDirection * ((movementMagnitude - dodgePositionPrevious) / Time.deltaTime);

        //Debug.Log ($"{movementMagnitude}");
        //Debug.Log ($"<color=red> {movementMagnitude - dodgePositionPrevious} </color>");
        //Debug.Log ($"<color=green> {(movementMagnitude - dodgePositionPrevious) / Time.deltaTime} </color>");

        dodgePositionPrevious = movementMagnitude;
        dodgeTimerPrevious = e.stateMachine.timerCurrentState;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition (transform.position, out hit, 0.5f, eg.agent.areaMask))
        {
            ES_Ragdoll ragdoll = GetComponent<ES_Ragdoll> ();
            ragdoll.entryVelocityInfluence = eBody.velocity;
            ragdoll.EnterRagdoll (false);

        }

        if (e.stateMachine.timerCurrentState > dodgeTime)
        {
                e.stateMachine.transitionState (GetComponent<ESG_Chase> ());
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