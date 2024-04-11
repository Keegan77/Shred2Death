using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostAbilityState : AbilityState
{
    public BoostAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        abilityBannedStateMap[GetType()] = new List<BehaviourState>()
        {
            player.halfPipeState,
            player.driftState,
            player.nosediveState,
        } ; // use this syntax to ban states
        
        abilityInputActions.Add(InputRouting.Instance.input.Player.Boost, new InputActionEvents()
        {
            onCanceled = ctx => stateMachine.SwitchState(player.intermediaryAbilityState)
        });
    }
    
    private float boostTimer;
    private Coroutine boostTimerCoroutine;
    private Coroutine rechargeBoostCoroutine;
    
    public override void Enter()
    {
        base.Enter();
        SubscribeInputs(abilityState:true);

        Debug.Log("Boost entered");
        player.movement.currentlyBoosting = true;
        if (boostTimer > player.playerData.boostDuration)
        {
            stateMachine.SwitchState(player.intermediaryAbilityState);
            return;
        }
        StartBoost();
    }
    
    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputs(abilityState:true);
        player.movement.currentlyBoosting = false;
        StopBoost();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    
    public void StartBoost() // subscribe to on input performed boost input
    {
        if (rechargeBoostCoroutine != null)
        {
            player.StopCoroutine(rechargeBoostCoroutine);
            currentlyRecharging = false;
            rechargeBoostCoroutine = null;
        }
        
        boostTimerCoroutine = player.StartCoroutine(BoostTimer());
        player.particleManager.JetStreamActive(true);
        player.particleManager.playerSpeedLines.Play();
    }
    
    bool currentlyRecharging;
    public void StopBoost() // subscribe this to on input canceled boost input cancel
    { //we don't need to reset speed here because the CalculateSpeedForce waits for this state to be done to continue
        if (boostTimerCoroutine != null)
        {
            player.StopCoroutine(boostTimerCoroutine);
            player.particleManager.JetStreamActive(false);
            player.particleManager.playerSpeedLines.Stop();
            player.movement.currentlyBoosting = false;
            boostTimerCoroutine = null;
        }
        if (currentlyRecharging) return;
        rechargeBoostCoroutine = player.StartCoroutine(RechargeBoost());
    }
    
    private IEnumerator BoostTimer()
    {
        player.movement.currentlyBoosting = true;
        player.movement.SetMovementSpeed(player.playerData.baseBoostSpeed);
        while (boostTimer < player.playerData.boostDuration)
        {
            boostTimer += Time.deltaTime;
            yield return null;
        }
        stateMachine.SwitchState(player.intermediaryAbilityState);
    }

    private IEnumerator RechargeBoost()
    {
        currentlyRecharging = true;
        while (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            yield return null;
        }
        currentlyRecharging = false;
    }
    
}