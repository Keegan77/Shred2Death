using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNosediveState : BehaviourState
{
    public PlayerNosediveState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        // put input here
        /*inputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onCanceled = ctx => stateMachine.SwitchState(player.airborneState) });*/
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.CheckAndSetSpline()});
    }

    public override void Enter()
    {
        base.Enter();
        player.particleManager.playerSpeedLines.Play();
        UnsubscribeInputs();
        SubscribeInputs();
        //player.extraGravityComponent.force = new Vector3(0, -player.playerData.fastFallGravity, 0);
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
        player.rb.AddForce(Vector3.down * player.playerData.noseDiveFallForce, ForceMode.Acceleration);
        if (player.rb.velocity.y < -Mathf.Abs(player.playerData.maxNoseDiveVelocity))
        {
            player.rb.velocity = new Vector3(player.rb.velocity.x, -Mathf.Abs(player.playerData.maxNoseDiveVelocity),
                                            player.rb.velocity.z);
        }
        RotateDownward();
        if (player.CheckGroundExtensions())
        {
            player.GetOrientationHandler().OrientFromExtensions();
        }
    }

    private void RotateDownward()
    {
        
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(player.inputTurningTransform.forward, Vector3.down) *
                               player.transform.rotation;

        player.transform.rotation = Quaternion.Lerp(startRotation, targetRot, Time.deltaTime * player.playerData.noseDiveRotationSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs();
        player.particleManager.playerSpeedLines.Stop();
        //player.extraGravityComponent.force = new Vector3(0, -player.playerData.extraGravity, 0);
    }
    
    public override void StateCollisionEnter(Collision collision)
    {
        base.StateCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Ground"))
        {
            stateMachine.SwitchState(player.skatingState);
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spline"))
        {
            player.CheckAndSetSpline();
        }
    }
    
    
}
