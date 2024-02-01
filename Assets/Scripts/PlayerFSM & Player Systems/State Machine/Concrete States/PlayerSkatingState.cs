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
        
        if (player.GetOrientationWithDownward() > 140 && player.GetOrientationWithDownward() < 190)
        {
            if (!player.CheckGround()) stateMachine.SwitchState(player.halfPipeState);
        } else if (!player.CheckGround())
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

}