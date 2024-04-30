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
    [Header("Ragdoll Parameters")]

    [Tooltip("How fast can the ragdoll be moving to still be considered stationary?")]
    [SerializeField] float thresholdStationary = 0.1f;

    [Tooltip("How long will the ragdoll stand still before standing up?")]
    [SerializeField] float timeRagdollRecovery = 2f;

    [Tooltip("If the enemy is in freefall for this long, they will start damaging themselves during AI update")]
    [SerializeField] float timeRagdollDestroy = 10f;
    private float timerRagdollDestroy = 0;

    [Range(1, 100), Tooltip("What percentage of the enemy's health do they lose if they are in free fall too long?")]
    [SerializeField] float damagePercentFreefall = 50;
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

        Vector3 spd = e.rb.velocity;
        e.SetRagdollEnabled (true);
        PushRagdoll (spd);

        ragdollStationary = false;
        timerRagdollDown = 0;

        e.bodyObject.transform.parent = null;
        ragdollSeparationObject.transform.parent = null;

        base.Enter ();
    }


    /// <summary>
    /// Disables the ragdoll and puts the enemy at the position the ragdoll was at.
    /// </summary>
    public override void Exit ()
    {
        e.animator.enabled = true;


        ragdollSeparationObject.transform.parent = ragdollRootObject.transform;
        e.bodyObject.transform.parent = e.transform;
        e.bodyObject.transform.localPosition = Vector3.zero; //Just in case the body object is not one of the nodes referenced by the state

        ragdollRootObject.transform.localPosition = offsetRagdollRoot;
        ragdollSeparationObject.transform.localPosition = offsetRagdollSeparation;
        ragdollMeshObject.transform.localPosition = offsetRagdollMesh;
        
        ragdollStationary = false;
        
        e.sensorsObject.SetActive (true);
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
            timerRagdollDestroy = 0;
        }
        else
        {
            timerRagdollDestroy += Time.deltaTime;
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
        base.AIUpdate ();

        ragdollStationary = rigidbodyRagdollTarget.velocity.magnitude <= thresholdStationary;

        if (timerRagdollDown >= timeRagdollRecovery )
        {

            RaycastHit hit;

            //Debug.Log ("ES_Ragdoll: Raycasting", this);
            if (Physics.Raycast (rigidbodyRagdollTarget.transform.position, Vector3.down, out hit, 5, LayerMask.GetMask ("Ground")))
            {
                //Debug.Log ("ES_Ragdoll: Raycast found", this);

                e.transform.position = hit.point;
                rigidbodyRagdollTarget.transform.localPosition = offsetRagdollRoot;

                e.stateMachine.transitionState (stateExit);
                return;
            }
            else
            {
                Debug.Log ("ES_Ragdoll: Raycast not found", this);
                Debug.LogWarning ($"{e} could not get up. It may have to be damaged via script", e);
            }

        }

        if (timerRagdollDestroy >= timeRagdollDestroy)
        {
            float damage = (damagePercentFreefall / 100f) * e.maxHealth;
            Debug.Log ($"DEAL DAMAGE: {damage} to {e.maxHealth} max health");
            
            e.TakeDamage (damage);
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
        
        if ( launch ) //Player has crashed in to the enemy
        {
            PushRagdoll (entryVelocityInfluence);

            e.audioPlayer.playClipRandom (e.audioImpact);
        }

        Debug.Log ($"{this.name}: Entered Ragdoll State");
    }

    /// <summary>
    /// If the enemy is tricked off of and enters ragdoll state with a custom influence.
    /// </summary>
    /// <param name="launchParams"></param>
    public void EnterRagdoll(Vector3 launchParams, bool playImpact)
    {
        e.stateMachine.transitionState (this);

        PushRagdoll (launchParams);

        if (playImpact) e.audioPlayer.playClipRandom (e.audioImpact);
    }

    void PushRagdoll(Vector3 v)
    {
        Rigidbody prb = Enemy.playerReference.GetComponent<Rigidbody> ();

        foreach (Rigidbody rb in e.ragdollBodies)
        {
            rb.AddForce (prb.velocity + v, ForceMode.VelocityChange);
        }
    }


    #endregion

    #region Setup

    Vector3 offsetRagdollRoot;
    Vector3 offsetRagdollSeparation;
    Vector3 offsetRagdollMesh;
    private void Start ()
    {
        offsetRagdollRoot = ragdollRootObject.transform.localPosition;
        offsetRagdollSeparation = ragdollSeparationObject.transform.localPosition;
        offsetRagdollMesh = ragdollMeshObject.transform.localPosition;
    }

    public void testDestroy ()
    {
        Destroy (e.gameObject);
    }
    
    /// <summary>
    /// Enmies are in the ragdoll state when they die, so their detached objects need to be cleared as well
    /// </summary>
    private void OnDestroy ()
    {
        //Debug.Log (ragdollRootObject);
        //Debug.Log (ragdollSeparationObject);
        if (ragdollRootObject != null) Destroy (ragdollRootObject.gameObject);
        if (ragdollSeparationObject != null) Destroy (ragdollSeparationObject);
        if ( e.bodyObject != null ) Destroy (e.bodyObject);
    }
    #endregion
}
