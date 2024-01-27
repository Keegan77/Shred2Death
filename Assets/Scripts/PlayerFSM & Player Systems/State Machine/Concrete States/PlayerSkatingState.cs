using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkatingState : PlayerState
{
    public PlayerSkatingState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
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
        
        if (!player.CheckGround())
        {
            stateMachine.SwitchState(player.airborneState);
        }

        if (InputRouting.Instance.GetDriftInput(alsoCheckForMoveInput:true))
        {
            
            stateMachine.SwitchState(player.driftState);
        }
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.CalculateTurnSharpness();
        player.SkateForward();
        player.DeAccelerate();
        player.OrientToSlope();
        if (InputRouting.Instance.GetMoveInput().y != 0) player.TurnPlayer();
    }
    
    public override void StateTriggerEnter(Collider other)
    {
        base.StateTriggerEnter(other);
    }
    
    public override void StateTriggerExit(Collider other)
    {
        base.StateTriggerExit(other);
    }

}
