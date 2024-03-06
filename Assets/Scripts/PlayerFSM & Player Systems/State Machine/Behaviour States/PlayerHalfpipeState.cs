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
    
    public override void Enter()
    {
        
        base.Enter();
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

    private void RotationInAir()
    {
        player.transform.Rotate(0,
            player.playerData.halfPipeAirTurnAmount * InputRouting.Instance.GetBumperInput().x * Time.fixedDeltaTime, 
            0, Space.Self);
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
