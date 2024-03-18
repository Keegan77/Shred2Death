using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHalfpipeState : PlayerState
{
    public PlayerHalfpipeState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onPerformed = ctx => stateMachine.SwitchState(player.nosediveState) });
        
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.CheckAndSetSpline()});
    }

    private GameObject closestHalfPipe;
    private int rotationIncrementsCompleted;
    public override void Enter()
    {
        player.GetComboHandler().SetPauseComboDrop(true);
        player.proceduralRigController.StartCoroutine(
            player.proceduralRigController.LerpWeightToValue
            (player.proceduralRigController.legRig,
                0,
                1f)
        );
        base.Enter();
        totalRotation = 0f;
        rotationIncrementsCompleted = 0;
        SubscribeInputs();
        
        player.GetOrientationHandler().SetOrientationSpeed(10f);
        
        foreach (var extrusionMesh in MeshContainerSingleton.Instance.extrusionMeshObjects)
        {
            extrusionMesh.GetComponent<MeshCollider>().enabled = true;
        }
        
        //player.StartCoroutine(player.ScaleCapsuleCollider(0.25f)); //EXPERIMENTAL - scales the player's collider to fit the half pipe
        player.GetMovementMethods().StopBoost();
        //closestHalfPipe = GetClosestHalfPipe();
    }

    public override void Exit()
    {
        base.Exit();
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
                                                   null));

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

        if (InputRouting.Instance.GetBoostInput())
        {
            player.movement.StartBoost();
            stateMachine.SwitchState(player.airborneState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HalfPipeAirBehaviour();
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
