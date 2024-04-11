using System.Collections.Generic;
using UnityEngine;

public class PlayerSkatingState : BehaviourState
{
    private PlayerMovementMethods movementMethods;
    private BowlMeshGenerator test;
    public PlayerSkatingState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        
        behaviourInputActions.Add(InputRouting.Instance.input.Player.DropIn, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                if (player.RaycastFromBowlCheckPoint().collider != null)
                {
                    player.dropinState.SetBowlSurfaceHit(player.RaycastFromBowlCheckPoint());
                    stateMachine.SwitchState(player.dropinState);
                }
            },
        });
        
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { 
                onPerformed = ctx =>
                {
                    player.capsuleFloater.SetRideHeight(player.capsuleFloater.crouchingRideHeight);
                },
                onCanceled = ctx =>
                {
                    if (!player.CheckGround()) return;
                    stateMachine.SwitchState(player.airborneState);
                    player.GetMovementMethods().OllieJump();
                }
            });
        
        //TODO: Change to play once when input is detected after it hasn't been detected for atleast one frame
        /*inputActions.Add(InputRouting.Instance.input.Player.MoveForwardButton, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                ActionEvents.PlayerSFXOneShot?.Invoke(SFXContainerSingleton.Instance.kickOffSounds[Random.Range(0, 
                    SFXContainerSingleton.Instance.kickOffSounds.Count)], 0);
            },
        });*/
        
    }

    private bool enteredHalfPipeSection;
    
    public override void Enter()
    {
        base.Enter();
        UnsubscribeInputs();
        SubscribeInputs();
        enteredHalfPipeSection = false;
        movementMethods = player.GetMovementMethods();
        //BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(1f, .2f));
        BulletTimeManager.Instance.ChangeBulletTime(1f);
    }
    
    public override void Exit()
    {
        UnsubscribeInputs();
        player.constantForce.relativeForce = new Vector3(0, 0, 0);
        player.capsuleFloater.SetRideHeight(player.capsuleFloater.standingRideHeight);
        base.Exit();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        float orientationWithDown = player.GetOrientationWithDownward() - 90;
        
        Vector3 newBackLeft = new Vector3(player.backLeftRayOrigin.x, player.backLeftRayOrigin.y, player.backLeftRayOrigin.z);
        Vector3 newBackRight = new Vector3(player.backRightRayOrigin.x, player.backRightRayOrigin.y, player.backRightRayOrigin.z);
        Vector3 rayOrigin = (newBackLeft + newBackRight) / 2;
        
        //convert ray origin to local space from player
        Vector3 rayOriginLocal = player.transform.InverseTransformPoint(rayOrigin);
        rayOriginLocal.z -= 1f;
        rayOrigin = player.transform.TransformPoint(rayOriginLocal);
        
        if (!player.CheckGround() && 
            Physics.Raycast(rayOrigin, -player.transform.up, out RaycastHit hit, 4, 1 << LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.Ignore)) // if we are facing upward and not on the ground, we go into halfpipe state
        {
            if (hit.collider.CompareTag("Ramp90")) stateMachine.SwitchState(player.halfPipeState);
        }
        else if (!player.CheckGround()) // if we are not facing upward, dont enter half pipe state, enter airborne
        {
            stateMachine.SwitchState(player.airborneState);
        }

        if (InputRouting.Instance.GetDriftInput() && player.rb.velocity.magnitude > 1f)
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
        movementMethods.SkateForward();
        movementMethods.DeAccelerate();
        if (player.CheckGround()) player.GetOrientationHandler().OrientToSlope();
        movementMethods.TurnPlayer();
    }
    
    public override void StateTriggerStay(Collider other)
    {
        base.StateTriggerStay(other);
        if (other.CompareTag("Ramp90")) enteredHalfPipeSection = true;

    }

}
