using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualieUltimateAbilityState : AbilityState
{
    GunPositionMover gunPositionMover;
    public DualieUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        abilityInputActions.Add(InputRouting.Instance.input.Player.Ability, new InputActionEvents()
        {
            onPerformed = ctx =>
            {
                stateMachine.SwitchState(player.intermediaryAbilityState);
            },
        });
    }
    
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs(abilityState:true);
        gunPositionMover = GameObject.FindObjectOfType<GunPositionMover>();
        Debug.Log("Dualie Ultimate Entered");
        gunPositionMover.SwitchToChristPosition();
        //player.gunSwitcher.SetRigTargetPoints(player.gunfireHandler.GetCurrentGunSceneData().GetAbilityTargets());
    }
    
    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs(abilityState:true);
        gunPositionMover.ResetTransformPositions();
    }
}