using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHalfpipeState : PlayerState
{
    public PlayerHalfpipeState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.CheckGround())
        {
            stateMachine.SwitchState(player.skatingState);
        }
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HalfPipeAirBehaviour();
        player.TurnPlayer();
    }
    /// <summary>
    /// Makes sure the player lands back on the half pipe. This is done by restricting the player's local Y velocity
    /// to 0, the player is sideways when doing a half pipe launch, so this will make sure the player ends up back
    /// on the half pipe slope. Should be ran every frame in FixedUpdate.
    /// </summary>
    public void HalfPipeAirBehaviour() 
    {
        Vector3 worldVelocity = player.rb.velocity;

        // converts the world velocity to local velocity
        Vector3 localVelocity = player.transform.InverseTransformDirection(worldVelocity);
        
        localVelocity.y = 0;

        // converts the modified local velocity back to world space
        Vector3 newWorldVelocity = player.transform.TransformDirection(localVelocity);
        
        player.rb.velocity = newWorldVelocity;
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
