using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkatingState : PlayerState
{
    private PlayerMovementMethods movementMethods;
    private BowlMeshGenerator test;
    public PlayerSkatingState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                if (player.GetComboHandler().GetStyleLevel() < player.playerData.groundedBoostStyleLevel) return;
                player.GetMovementMethods().StartBoost();
            },
            onCanceled = ctx =>
            {
                if (player.GetComboHandler().GetStyleLevel() < player.playerData.groundedBoostStyleLevel) return;
                player.GetMovementMethods().StopBoost();
            }
        });
        
        inputActions.Add(InputRouting.Instance.input.Player.TimeShiftDebug, new InputActionEvents
        {
            onPerformed = ctx => BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(0.2f, .2f)),
            onCanceled = ctx => BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(1f, .2f)),
        });
        
        inputActions.Add(InputRouting.Instance.input.Player.DropIn, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                if (player.RaycastFromBowlCheckPoint().collider != null)
                {
                    player.dropinState.SetBowlSurfaceHit(player.RaycastFromBowlCheckPoint());
                    stateMachine.SwitchState(player.dropinState);
                }
            },
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
        BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(1f, .2f));
    }
    
    public override void Exit()
    {
        UnsubscribeInputs();
        player.constantForce.relativeForce = new Vector3(0, 0, 0);
        base.Exit();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!player.CheckGround() && !InputRouting.Instance.GetBoostInput() && !player.GetOrientationWithDownward().IsInRangeOf(70, 110)) // if we are facing upward and not boosting and not on the ground, we go into halfpipe state
        {
            stateMachine.SwitchState(player.halfPipeState);
        }
        else if (!player.CheckGround()) // if we are not facing upward, dont enter half pipe state, enter airborne
        {
            stateMachine.SwitchState(player.airborneState);
        }

        if (InputRouting.Instance.GetDriftInput(alsoCheckForMoveInput:true))
        {
            stateMachine.SwitchState(player.driftState);
        }

        if (player.CheckGround())
        {
            player.constantForce.relativeForce = new Vector3(0, -20, 0);
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
