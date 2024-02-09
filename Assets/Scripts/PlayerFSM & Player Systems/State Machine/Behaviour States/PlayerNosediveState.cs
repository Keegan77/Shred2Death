using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNosediveState : PlayerState
{
    public PlayerNosediveState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        // put input here
        /*inputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onCanceled = ctx => stateMachine.SwitchState(player.airborneState) });*/
    }

    public override void Enter()
    {
        base.Enter();
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
        if (player.rb.velocity.y < -player.playerData.maxNoseDiveVelocity)
        {
            player.rb.velocity = new Vector3(player.rb.velocity.x, -player.playerData.maxNoseDiveVelocity, player.rb.velocity.z);
        }
        RotateDownward();
        if (player.CheckGroundExtensions())
        {
            player.GetOrientationHandler().OrientFromExtensions();
        }

        //player.transform.rotation = Quaternion.LookRotation(player.inputTurningTransform.forward, player.inputTurningTransform.forward);
    }

    private void RotateDownward()
    {
        float speed = 2;
        
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(player.inputTurningTransform.forward, Vector3.down) *
                               player.transform.rotation;

        player.transform.rotation = Quaternion.Lerp(startRotation, targetRot, Time.deltaTime * speed);
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs();
        //player.extraGravityComponent.force = new Vector3(0, -player.playerData.extraGravity, 0);
    }
    
    
}