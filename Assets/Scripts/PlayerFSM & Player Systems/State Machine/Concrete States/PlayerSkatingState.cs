using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkatingState : PlayerState
{
    public PlayerSkatingState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    private bool enteredHalfPipeSection;
    
    public override void Enter()
    {
        base.Enter();
        enteredHalfPipeSection = false;
    }
    
    public override void Exit()
    {
        base.Exit();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (enteredHalfPipeSection)
        {
            if (!player.CheckGround() && !InputRouting.Instance.GetBoostInput() && !player.GetOrientationWithDownward().IsInRangeOf(70, 110))
            {
                stateMachine.SwitchState(player.halfPipeState);
            }
            else if (!player.CheckGround() && InputRouting.Instance.GetBoostInput())
            {
                stateMachine.SwitchState(player.airborneState);
            }
            
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
    
    public override void StateTriggerStay(Collider other)
    {
        base.StateTriggerStay(other);
        if (other.CompareTag("Ramp90")) enteredHalfPipeSection = true;

    }

}
