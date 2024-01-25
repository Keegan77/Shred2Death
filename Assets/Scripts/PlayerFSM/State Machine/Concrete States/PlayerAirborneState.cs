using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    
    public override void Exit()
    {
        base.Exit();
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
        
        player.ReOrient();
        player.TurnPlayer();
    }
    
    public override void StateTriggerEnter(Collider other)
    {
        base.StateTriggerEnter(other);

    }
    
    public override void StateTriggerStay(Collider other)
    {
        base.StateTriggerStay(other);
        if (other.CompareTag("Ramp90"))
        {
            stateMachine.SwitchState(player.halfPipeState);
        }
    }
    
    public override void StateTriggerExit(Collider other)
    {
        base.StateTriggerExit(other);
    }  
}
