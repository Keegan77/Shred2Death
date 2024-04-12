using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// When enemies die or get boosted into by a player, they will begin to ragdoll.
/// 
/// While in the state
/// </summary>
public class ES_Ragdoll : Enemy_State
{
    #region Parameters
    [Tooltip("How fast can the ragdoll be moving to still be considered stationary?")]
    [SerializeField] float thresholdStationary = 0.1f;

    [Tooltip("How long will the ragdoll stand still before standing up?")]
    [SerializeField] float timeRagdollRecovery = 2f;
    #endregion

    #region References
    [Header("Ragdoll State")]
    [Tooltip("State to transition to when leaving the ragdoll")]
    public Enemy_State stateExit;

    [Tooltip("Assign this to the object that will recieve the force of entering the state")]
    public Rigidbody objectRagdollTarget;

    [SerializeField] GameObject ragdollRootObject;
    [SerializeField] GameObject ragdollSeparationObject;
    [SerializeField] GameObject ragdollMeshObject;
    #endregion

    #region Script Variables

    //When still, how long has the ragdoll been still for?
    float timerRagdollDown;

    bool _still;
    bool ragdollStationary { get { return _still; } set { _still = value; timerRagdollDown = Time.time; } }
    #endregion

    #region StateMachine
    public override void Enter ()
    {
        e.sensorsObject.SetActive(false);

        e.SetRagdollEnabled (true);
        ragdollStationary = false;

        base.Enter ();


        e.bodyObject.transform.parent = null;
        ragdollSeparationObject.transform.parent = null;

    }


    /// <summary>
    /// Disables the ragdoll and puts the enemy at the position the ragdoll was at.
    /// </summary>
    public override void Exit ()
    {
        ragdollSeparationObject.transform.parent = ragdollRootObject.transform;
        e.bodyObject.transform.parent = e.transform;
        
        ragdollStationary = false;
        RaycastHit hit;

        if (Physics.Raycast(objectRagdollTarget.transform.position, Vector3.down, out hit, 5, LayerMask.GetMask ("Ground")) )
        {
            e.transform.position = hit.point;
            objectRagdollTarget.transform.localPosition = offsetRagdollTarget;

        }
        else
        {
            Debug.Log ("ES_Ragdoll: Raycast not found", this);
        }

        e.sensorsObject.SetActive (true);
        e.bodyObject.transform.position = e.transform.position;
        e.SetRagdollEnabled (false);
        base.Exit ();

    }

    public override void machinePhysics ()
    {
        base.machinePhysics ();
        e.bodyObject.transform.position = ragdollSeparationObject.transform.position;
    }

    /// <summary>
    /// Decides when to get back up. by keeping a timer and trakc of movement.
    /// If the ragdoll has stopped moving, start the timer to see if it sits still for long enough.
    /// 
    /// Once this happens, exit to a designated state.
    /// </summary>
    public override void AIUpdate ()
    {
        //base.AIUpdate ();

        if ( ragdollStationary )
        {
            timerRagdollDown += Time.deltaTime;

            if (timerRagdollDown >= timeRagdollRecovery )
            {
                e.stateMachine.transitionState (stateExit);
                return;
            }

            if (objectRagdollTarget.velocity.magnitude >= thresholdStationary )
            {
                ragdollStationary = false;
            }
        }
        else
        {
            if (objectRagdollTarget.velocity.magnitude <= thresholdStationary )
            {
                ragdollStationary = true;
            }
        }
    }
    #endregion

    #region SCRIPT FUNCTIONS
    /// <summary>
    /// Invoked by a sensor that checks to see if the player boosts into the enemy.
    /// 
    /// Set the entryVelocity ahead of time if launch is false
    /// </summary>
    /// <param name="launch"></param>
    /// 
    public Vector3 entryVelocity;
    public void EnterRagdoll (bool launch)
    {
        e.stateMachine.transitionState (this);

        if ( launch )
        {
            objectRagdollTarget.AddForce (new Vector3 (0, 100, -10), ForceMode.VelocityChange);
        }
        else
        {
            foreach(Rigidbody rb in e.ragdollBodies)
            {
                rb.AddForce (entryVelocity, ForceMode.VelocityChange);

            }
            Debug.Log (entryVelocity);
        }

        Debug.Log ($"{this.name}: Entered Ragdoll State");
    }


    #endregion

    #region Setup

    Vector3 offsetRagdollTarget;
    private void Start ()
    {
        offsetRagdollTarget = objectRagdollTarget.transform.localPosition;
    }
    #endregion
}
