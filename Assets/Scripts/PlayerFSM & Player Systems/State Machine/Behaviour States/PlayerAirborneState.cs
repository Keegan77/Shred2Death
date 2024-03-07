using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    private bool coolingDown;
    private Coroutine coolDownCoroutine;
    public PlayerAirborneState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents 
            { onPerformed = ctx => player.CheckAndSetSpline()});
        
        inputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents
        {
            onPerformed = ctx =>
            {
                if (coolingDown) return;
                if (player.GetComboHandler().GetStyleLevel() < player.playerData.airBoostStyleLevel) return;
                player.rb.velocity = Vector3.zero;
                AddAirForce(100, ForceMode.Impulse, accountForForwardInput:false);
                coolDownCoroutine = player.StartCoroutine(CoolDownAirBoost());
            },
        });
        
        inputActions.Add(InputRouting.Instance.input.Player.Nosedive, new InputActionEvents 
            { onPerformed = ctx => stateMachine.SwitchState(player.nosediveState) });
    }
    
    private IEnumerator CoolDownAirBoost()
    {
        coolingDown = true;
        yield return new WaitForSeconds(player.playerData.airBoostCooldownSeconds);
        coolingDown = false;
    }

    TrickComboHandler comboHandler;
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs();
        ActionEvents.OnPlayBehaviourAnimation?.Invoke("Idle");
        comboHandler = player.gameObject.GetComponent<TrickComboHandler>();
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs();
        if (coolDownCoroutine != null)
        {
            player.StopCoroutine(coolDownCoroutine);
            coolingDown = false;
        }
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
        if (player.GetComboHandler().GetStyleLevel() < player.playerData.groundedBoostStyleLevel) return;
        AddAirForce(player.playerData.airForwardForce, ForceMode.Acceleration);
        
    }

    private void TiltPlayerWithVelocity()
    {
        float yVelocity = -player.rb.velocity.y;
        float tiltAngle = Mathf.Clamp(yVelocity, -player.playerData.airTiltRange, player.playerData.airTiltRange);

        // Create a Quaternion for the tilt rotation.
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, player.transform.eulerAngles.y, player.transform.eulerAngles.z);
        
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, tiltRotation, Time.deltaTime * 5);
    }
    
    private void AddAirForce(float forceAmount, ForceMode forceMode, bool accountForForwardInput = true)
    {
        Vector3 worldForward = player.inputTurningTransform.forward;
        Vector3 localForward = player.inputTurningTransform.InverseTransformDirection(worldForward);
        
        localForward.x = 0;
        localForward.y = 0;

        Vector3 newWorldForward = player.inputTurningTransform.TransformDirection(localForward);
        newWorldForward = newWorldForward.normalized;
        
        var forwardInput = InputRouting.Instance.GetMoveInput().y;
        if (accountForForwardInput)
        {
            if (forwardInput > 0)
            {
                forwardInput = 1;
            }
        }
        else
        {
            forwardInput = 1;
        }

        
        player.rb.AddForce(newWorldForward * (forceAmount * forwardInput), forceMode);
        
    }

}
