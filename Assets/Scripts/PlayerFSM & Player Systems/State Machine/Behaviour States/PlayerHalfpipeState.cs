using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHalfpipeState : PlayerState
{
    public PlayerHalfpipeState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    private GameObject closestHalfPipe;
    
    public override void Enter()
    {
        base.Enter();

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
        
        if (player.CheckGround() && player.rb.velocity.y < 0) // if we detect the ground layer and are going downward
        {
            stateMachine.SwitchState(player.skatingState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HalfPipeAirBehaviour();
        player.GetMovementMethods().TurnPlayer();
        if (player.rb.velocity.y < 0 && player.CheckGroundExtensions()) 
            player.GetOrientationHandler().OrientFromExtensions(); // the if statement prevents accidental landing
                                                                     // rotation when the player is still in the air
    }

    public void HalfPipeAirBehaviour()
    {
        player.CheckGround("BowlMesh");
        player.GetOrientationHandler().OrientToSlope();
        //player.rb.SetLocalAxisVelocity(Vector3.up, 0);
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
