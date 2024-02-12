using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.CheckAndSetSpline()});
        
        inputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents
        {
            onPerformed = ctx => player.GetMovementMethods().StartBoost(),
            onCanceled = ctx => player.GetMovementMethods().StopBoost()
        });
        
        inputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onPerformed = ctx => stateMachine.SwitchState(player.nosediveState) });
    }
    
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs();
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs();
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
        
        TiltPlayerWithVelocity();
        
        player.GetOrientationHandler().ReOrient();
        player.GetMovementMethods().TurnPlayer();
        AddAirForce();
        
    }

    private void TiltPlayerWithVelocity()
    {
        float yVelocity = -player.rb.velocity.y;
        float tiltAngle = Mathf.Clamp(yVelocity, -player.playerData.airTiltRange, player.playerData.airTiltRange);

        // Create a Quaternion for the tilt rotation.
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, player.transform.eulerAngles.y, player.transform.eulerAngles.z);
        
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, tiltRotation, Time.deltaTime * 5);
    }


    private void AddAirForce()
    {
        Vector3 worldForward = player.inputTurningTransform.forward;
        Vector3 localForward = player.inputTurningTransform.InverseTransformDirection(worldForward);
        
        localForward.x = 0;
        localForward.y = 0;

        Vector3 newWorldForward = player.inputTurningTransform.TransformDirection(localForward);
        newWorldForward = newWorldForward.normalized;
        
        var forwardInput = InputRouting.Instance.GetMoveInput().y;
        if (forwardInput < 0)
        {
            forwardInput = 0;
        }
        
        player.rb.AddForce(newWorldForward * (player.playerData.airForwardForce * forwardInput), ForceMode.Acceleration);
        
    }
    

}
