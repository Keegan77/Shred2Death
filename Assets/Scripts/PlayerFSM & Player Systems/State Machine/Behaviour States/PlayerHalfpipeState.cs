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
        //player.StartCoroutine(player.ScaleCapsuleCollider(0.25f)); //EXPERIMENTAL - scales the player's collider to fit the half pipe
        player.GetMovementMethods().StopBoost();
        closestHalfPipe = GetClosestHalfPipe();
        Debug.Log($"Closest half pipe: {closestHalfPipe.name}");
    }

    public override void Exit()
    {
        base.Exit();
        //player.StartCoroutine(player.ScaleCapsuleCollider(player.GetOriginalColliderRadius()));
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.CheckGround())
        {
            stateMachine.SwitchState(player.skatingState);
        }
    }

    private GameObject GetClosestHalfPipe()
    {
        GameObject closestHalfPipe;
        
        var enumerator = VertexContainer.Instance.objectVerticeMap.Keys.GetEnumerator();
        enumerator.MoveNext();
        closestHalfPipe = enumerator.Current;
        
        foreach (var halfPipe in VertexContainer.Instance.objectVerticeMap.Keys)
        {
            if (Vector3.Distance(player.transform.position, halfPipe.transform.position) <
                Vector3.Distance(player.transform.position, closestHalfPipe.transform.position))
            {
                closestHalfPipe = halfPipe;
            }
        }

        return closestHalfPipe;
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
    /// <summary>
    /// Makes sure the player lands back on the half pipe. This is done by restricting the player's local Y velocity
    /// to 0, the player is sideways when doing a half pipe launch, so this will make sure the player ends up back
    /// on the half pipe slope. Should be ran every frame in FixedUpdate.
    /// </summary>
    public void HalfPipeAirBehaviour() 
    {
        player.rb.SetLocalAxisVelocity(Vector3.up, 0);
    }
    
    public override void StateTriggerExit(Collider other)
    {
        base.StateTriggerExit(other);
        Debug.Log("Exited half pipe volume");
        if (other.CompareTag("Ramp90"))
        {
            if (player.CheckGround()) stateMachine.SwitchState(player.skatingState);
            
            else stateMachine.SwitchState(player.airborneState);
            
        }
    }
    
    
}
