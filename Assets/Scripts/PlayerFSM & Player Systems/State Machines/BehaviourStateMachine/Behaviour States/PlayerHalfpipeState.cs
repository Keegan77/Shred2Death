using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHalfpipeState : BehaviourState
{
    public PlayerHalfpipeState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onPerformed = ctx => stateMachine.SwitchState(player.nosediveState) });
        
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.CheckAndSetSpline()});
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                //player.GetMovementMethods().StartBoost();
                stateMachine.SwitchState(player.airborneState);
            },
        });
    }

    private GameObject closestHalfPipe;
    private int rotationIncrementsCompleted;
    private Quaternion initRotation;
    private Coroutine lerpRigRoutine;
    public override void Enter()
    {
        player.GetComboHandler().SetPauseComboDrop(true);
        lerpRigRoutine = player.proceduralRigController.StartCoroutine(
            player.proceduralRigController.LerpWeightToValue
            (player.proceduralRigController.legRig,
                0,
                1f)
        );
        base.Enter();
        totalRotation = 0f;
        rotationIncrementsCompleted = 0;
        UnsubscribeInputs();
        SubscribeInputs();
        
        player.GetOrientationHandler().SetOrientationSpeed(20f);
        
        foreach (var extrusionMesh in MeshContainerSingleton.Instance.extrusionMeshObjects)
        {
            extrusionMesh.GetComponent<MeshCollider>().enabled = true;
        }
        
        //player.StartCoroutine(player.ScaleCapsuleCollider(0.25f)); //EXPERIMENTAL - scales the player's collider to fit the half pipe
        initRotation = player.transform.rotation;
    }

    public override void Exit()
    {
        base.Exit();
        if (lerpRigRoutine != null) player.proceduralRigController.StopCoroutine(lerpRigRoutine);
        player.proceduralRigController.StartCoroutine(
            player.proceduralRigController.LerpWeightToValue
            (player.proceduralRigController.legRig,
                1,
                .1f)
        );
        player.GetComboHandler().SetPauseComboDrop(false);
        Debug.Log($"Total rotation style: {rotationIncrementsCompleted * 180}");
        ActionEvents.OnTrickCompletion?.Invoke(new Trick($"Rotation trick: " +
                                                         $"{rotationIncrementsCompleted * 180}",
                                                   rotationIncrementsCompleted * 6,
                                                  2 * rotationIncrementsCompleted,
                                                0.1f,
                                                   null,
                                                   0));

        player.constantForce.relativeForce = new Vector3(0, 0, 0);
        UnsubscribeInputs();
        player.GetOrientationHandler().ResetOrientationSpeed();
        foreach (var extrusionMesh in MeshContainerSingleton.Instance.extrusionMeshObjects)
        {
            extrusionMesh.GetComponent<MeshCollider>().enabled = false;
        }
        //player.StartCoroutine(player.ScaleCapsuleCollider(player.GetOriginalColliderRadius()));
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.CheckGround() && player.rb.velocity.y <= 0) // if we detect the ground layer and are going downward
        {
            stateMachine.SwitchState(player.skatingState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HalfPipeAirBehaviour();
        RotateWithYVelocity();
        RotationInAir();
        //player.GetMovementMethods().TurnPlayer();
        if (player.rb.velocity.y < 0 && player.CheckGroundExtensions()) 
            player.GetOrientationHandler().OrientFromExtensions(); // the if statement prevents accidental landing
                                                                     // rotation when the player is still in the air
        if (player.rb.velocity.y > 0 && player.CheckGround())
        {
             player.movement.SkateForward();
        }

        if (player.CheckGround("BowlMesh"))
        {
            player.constantForce.relativeForce = new Vector3(0, -5, 0);
        }
        else
        {
            player.constantForce.relativeForce = new Vector3(0, 0, 0);
        }
        
        //player.rb.SetLocalAxisVelocity(player..up, 0);
        
    }
    private float previousXRotation = 0f;
    private float totalRotation = 0f;
    
    private void RotationInAir()
    {
        float rotationThisFrame = player.playerData.halfPipeAirTurnAmount * InputRouting.Instance.GetBumperInput().x * Time.fixedDeltaTime;
        player.transform.Rotate(0, rotationThisFrame, 0, Space.Self);

        totalRotation += rotationThisFrame;

        if (Mathf.Abs(totalRotation) >= 180f)
        {
            rotationIncrementsCompleted++;
            totalRotation = 0f;
        }
    }

    private void RotateWithYVelocity()
    {
        // Get the y velocity
        float yVelocity = player.rb.velocity.y;

        // Convert the y velocity to a percentage between -30 and 30
        float t = Mathf.InverseLerp(30, -30, yVelocity);
        // Calculate the target rotation
        Quaternion targetRot = Quaternion.LookRotation(Vector3.down, player.transform.up);


        if (InputRouting.Instance.GetBumperInput().magnitude > .1f) return;
        // Interpolate between the current rotation and the target rotation based on the percentage
        player.transform.rotation = Quaternion.Lerp(initRotation, targetRot, t);
    }
    

    private IEnumerator LerpDefaultRotation()
    {
        float t = 0;
        
        while (t < 1)
        {
            t += Time.deltaTime;
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(90, player.transform.rotation.y, player.transform.rotation.z), t);
        }

        yield return null;
    }

    public void HalfPipeAirBehaviour()
    {
        player.CheckGround("BowlMesh");
        player.GetOrientationHandler().OrientToSlope();
        //if (player.rb.GetLocalVelocity().y > 0) player.rb.SetLocalAxisVelocity(Vector3.up, 0);
    }
    
    public override void StateTriggerExit(Collider other)
    {
        base.StateTriggerExit(other);
        /*Debug.Log("Exited half pipe volume");
        if (other.CompareTag("Ramp90"))
        {
            if (player.CheckGround()) stateMachine.SwitchState(player.skatingState);
            
            else stateMachine.SwitchState(player.airborneState);
            
        }*/
    }
    
    
}
