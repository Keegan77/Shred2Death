using System.Collections;
using System.Collections.Generic;
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
    public Rigidbody rigidbodyRagdollTarget;

    [Tooltip("Wha tthe separation object is parented to")]
    [SerializeField] GameObject ragdollRootObject;

    [Tooltip("Game Object that ragdoll joints stem from")]
    [SerializeField] GameObject ragdollSeparationObject;

    [SerializeField] GameObject ragdollMeshObject;
    #endregion

    #region Script Variables

    //When still, how long has the ragdoll been still for?
    float timerRagdollDown = 0;

    bool _still = false;
    bool ragdollStationary { get { return _still; } set { _still = value;} }
    #endregion

    #region StateMachine
    public override void Enter ()
    {
        e.sensorsObject.SetActive(false);

        e.SetRagdollEnabled (true);
        ragdollStationary = false;
        timerRagdollDown = 0;

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
        ragdollRootObject.transform.position = e.transform.position;
        
        ragdollStationary = false;
        

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

    public override void machineUpdate ()
    {
        if (ragdollStationary)
        {
            timerRagdollDown += Time.deltaTime;
        }
        else
        {
            timerRagdollDown = 0;
        }
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

        ragdollStationary = rigidbodyRagdollTarget.velocity.magnitude <= thresholdStationary;

        if (timerRagdollDown >= timeRagdollRecovery )
        {

            RaycastHit hit;

            //Debug.Log ("ES_Ragdoll: Raycasting", this);
            if (Physics.Raycast (rigidbodyRagdollTarget.transform.position, Vector3.down, out hit, 5, LayerMask.GetMask ("Ground")))
            {
                //Debug.Log ("ES_Ragdoll: Raycast found", this);

                e.transform.position = hit.point;
                rigidbodyRagdollTarget.transform.localPosition = offsetRagdollTarget;

                e.stateMachine.transitionState (stateExit);
            }
            else
            {
                Debug.Log ("ES_Ragdoll: Raycast not found", this);
                Debug.LogWarning ($"{e} could not get up. It may have to be damaged via script", e);
            }

        }

    }
    #endregion

    #region SCRIPT FUNCTIONS
    public Vector3 entryVelocityInfluence;

    /// <summary>
    /// Invoked by a sensor that checks to see if the player boosts into the enemy.
    /// 
    /// If Launch is false, Set the entryVelocity vector ahead of time
    /// </summary>
    /// <param name="launch"></param>
    /// 
    public void EnterRagdoll (bool launch)
    {
        e.stateMachine.transitionState (this);
        
        if ( launch )
        {
            Rigidbody prb = Enemy.playerReference.GetComponent<Rigidbody> ();

            foreach (Rigidbody rb in e.ragdollBodies)
            {
                rb.AddForce (prb.velocity + entryVelocityInfluence, ForceMode.VelocityChange);
            }

            //objectRagdollTarget.AddForce (prb.velocity + entryVelocityInfluence, ForceMode.VelocityChange);
        }

        Debug.Log ($"{this.name}: Entered Ragdoll State");
    }


    #endregion

    #region Setup

    Vector3 offsetRagdollTarget;
    private void Start ()
    {
        offsetRagdollTarget = rigidbodyRagdollTarget.transform.localPosition;
    }

    public void testDestroy ()
    {
        Destroy (e.gameObject);
    }
    private void OnDestroy ()
    {

        if (ragdollRootObject) Destroy (ragdollRootObject.transform.parent.gameObject);
        if (ragdollSeparationObject) Destroy (ragdollSeparationObject);
    }
    #endregion
}
