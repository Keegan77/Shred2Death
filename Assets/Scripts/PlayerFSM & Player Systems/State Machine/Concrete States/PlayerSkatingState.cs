using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkatingState : PlayerState
{
    private PlayerMovementMethods movementMethods;
    public PlayerSkatingState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.GetMovementMethods().OllieJump()});
        
        inputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents
        {
            onPerformed = ctx => player.GetMovementMethods().StartBoost(),
            onCanceled = ctx => player.GetMovementMethods().StopBoost()
        });
    }

    private bool enteredHalfPipeSection;
    
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs();
        enteredHalfPipeSection = false;
        movementMethods = player.GetMovementMethods();
    }
    
    public override void Exit()
    {
        UnsubscribeInputs();
        base.Exit();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (enteredHalfPipeSection)
        {
            if (!player.CheckGround() && !InputRouting.Instance.GetBoostInput() && !player.GetOrientationWithDownward().IsInRangeOf(70, 110)) // if we are facing upward and not boosting and not on the ground, we go into halfpipe state
            {
                stateMachine.SwitchState(player.halfPipeState);
            }
            else if (!player.CheckGround()) // if we are not facing upward, dont enter half pipe state, enter airborne
            {
                stateMachine.SwitchState(player.airborneState);
            }
            
        } else if (!player.CheckGround()) // if we haven't entered a half pipe section and we dont detect ground
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
        movementMethods.CalculateTurnSharpness();
        movementMethods.SkateForward();
        movementMethods.DeAccelerate();
        if (player.CheckGround()) player.GetOrientationHandler().OrientToSlope();
        if (InputRouting.Instance.GetMoveInput().y != 0) movementMethods.TurnPlayer();
    }
    
    public override void StateTriggerStay(Collider other)
    {
        base.StateTriggerStay(other);
        if (other.CompareTag("Ramp90")) enteredHalfPipeSection = true;

    }

}
