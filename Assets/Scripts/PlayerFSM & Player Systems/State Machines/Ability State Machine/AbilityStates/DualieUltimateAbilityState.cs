using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualieUltimateAbilityState : AbilityState
{
    GunPositionMover gunPositionMover;
    GunfireHandler gunfireHandler;
    GunSwitcher gunSwitcher;
    PlayerRootMover playerRootMover;
    Coroutine autoFireCoroutine;
    Coroutine rotateBackCoroutine;
    private float turnSpeed = 200;
    float abilityLifetime = 5f;
    private float shotCooldown = 0.05f;
    private float currentLifetime = 0f;
    private Vector3 recoil = new Vector3(10, 12.5f, 5);
    public DualieUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        gunSwitcher = player.gunSwitcher;
        gunfireHandler = player.gunfireHandler;
        playerRootMover = player.rootMover;
        abilityInputActions.Add(InputRouting.Instance.input.Player.Fire, new InputActionEvents()
        {
            onStarted = ctx =>
            {
                rotateBackCoroutine = playerRootMover.StartCoroutine(playerRootMover.RotateBackToZero());
                gunfireHandler.ShootFromGunForward(false);
                foreach (var recoil in gunfireHandler.GetCurrentGunSceneData().GetRecoilObjects())
                {
                    recoil.ResetStartRotation();
                }
                gunPositionMover.ResetTransformPositions();
                player.proceduralRigController.SetWeightToValue(player.proceduralRigController.headAndChestRig, 1);
                gunSwitcher.SetRigTargetPoints(gunfireHandler.GetCurrentGunSceneData().GetAllTargets());
            },
            onCanceled = ctx =>
            {
                if (rotateBackCoroutine != null) playerRootMover.StopCoroutine(rotateBackCoroutine);
                if (stateMachine.currentAbilityState == this)
                {
                    gunfireHandler.ShootFromGunForward(true);
                    player.proceduralRigController.SetWeightToValue(player.proceduralRigController.headAndChestRig, 0);
                    gunPositionMover.SwitchToChristPosition();
                }
            }
        });
    }
    
    public override void Enter()
    {
        base.Enter();
        currentLifetime = 0;
        gunfireHandler.DisablePlayerFire(true); //we no longer want player input to determine gunfire
        SubscribeInputs(abilityState:true);
        gunPositionMover = GameObject.FindObjectOfType<GunPositionMover>();
        Debug.Log("Dualie Ultimate Entered");
        gunPositionMover.SwitchToChristPosition();
        //player.gunSwitcher.SetRigTargetPoints(player.gunfireHandler.GetCurrentGunSceneData().GetAbilityTargets());
        autoFireCoroutine = player.StartCoroutine(AutoFire());
        player.proceduralRigController.SetWeightToValue(player.proceduralRigController.headAndChestRig, 0);
        if (rotateBackCoroutine != null) player.StopCoroutine(rotateBackCoroutine);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        currentLifetime += Time.deltaTime;

        if (InputRouting.Instance.GetFireHeld()) return;
        player.rootMover.transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
    }

    private IEnumerator AutoFire()
    {
        gunfireHandler.ShootFromGunForward(true);
        gunfireHandler.SetGunRecoil(recoil);
        while (currentLifetime < abilityLifetime)
        {
            gunfireHandler.ExecuteGunshot();
            yield return new WaitForSeconds(shotCooldown);
            gunfireHandler.SwitchAlternateFire();
        }
        stateMachine.SwitchState(player.intermediaryAbilityState);
    }
    
    public override void Exit()
    {
        base.Exit();
        if (autoFireCoroutine != null) player.StopCoroutine(autoFireCoroutine);
        UnsubscribeInputs(abilityState:true);
        ActionEvents.MakePlayerLookMouse?.Invoke();
        gunfireHandler.ShootFromGunForward(false);
        gunfireHandler.DisablePlayerFire(false);
        gunfireHandler.SetGunRecoil(new Vector3(gunfireHandler.GetCurrentGunData().gunRecoilX,
                                                gunfireHandler.GetCurrentGunData().gunRecoilY, 
                                                gunfireHandler.GetCurrentGunData().gunRecoilZ));
        gunPositionMover.ResetTransformPositions();
        gunSwitcher.SetRigTargetPoints(gunfireHandler.GetCurrentGunSceneData().GetAllTargets());
        rotateBackCoroutine = playerRootMover.StartCoroutine(playerRootMover.RotateBackToZero());
        player.proceduralRigController.SetWeightToValue(player.proceduralRigController.headAndChestRig, 1);
    }
}