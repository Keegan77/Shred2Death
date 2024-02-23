using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriftState : PlayerState
{
    // Start is called before the first frame update
    public PlayerDriftState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    private enum DriftPhase
    {
        Light,
        BoostOne,
        BoostTwo,
        BoostThree
    }
    private DriftPhase currentDriftPhase;

    private float timer;
    private float driftSpeedBoost;

    private float rotationAmount;
    private bool currentlyRotating;
    private bool exitQueued;
    private float current;
    private float driftDirection; // -1 is left, 1 is right, 0 means no drift occurs
    public override void Enter()
    {
        base.Enter();
        player.GetMovementMethods().StopBoost();
        timer = 0;
        driftSpeedBoost = 0;
        rotationAmount = player.playerData.driftRotationalOffset;
        exitQueued = false;
        driftDirection = Mathf.Sign(InputRouting.Instance.GetMoveInput().x);
        current = 0;
        player.StartCoroutine(DriftRotationY(player.playerModelTransform, driftDirection * rotationAmount, player.playerData.playerModelRotationSpeed));
        player.StartCoroutine(DriftPhaseController());
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!player.CheckGround())
        {
            player.GetOrientationHandler().ReOrient();
        }
        else
        {
            player.GetOrientationHandler().OrientToSlope();
        }
        Drift();
        
        player.GetMovementMethods().DeAccelerate();
        //.Log(driftSpeedBoost);
        timer += Time.deltaTime;
    }

    /// <summary>
    /// Determines the current drift phase, used every frame in update during the drift state.
    /// </summary>
    private IEnumerator DriftPhaseController()
    {
        currentDriftPhase = DriftPhase.Light;

        yield return new WaitUntil(() => timer > player.playerData.lightDriftTime);
        currentDriftPhase = GetNextDriftPhase();
        driftSpeedBoost = player.playerData.baseDriftBoost;
        timer = 0;
        while (GetNextDriftPhase() != currentDriftPhase) //while there is another drift phase to go to
        {
            yield return new WaitUntil(() => timer > player.playerData.driftPhaseTime);
            currentDriftPhase = GetNextDriftPhase();
            driftSpeedBoost += player.playerData.driftBoostAdditive;
            timer = 0;
        }
        
    }
    
    /// <returns>The next drift phase in the list, dependent on currentDriftPhase. If the next drift phase
    /// does not exist, this will return the current drift phase.</returns>
    private DriftPhase GetNextDriftPhase()
    {
        DriftPhase nextDriftPhase = currentDriftPhase + 1;
        if (Enum.IsDefined(typeof(DriftPhase), nextDriftPhase))
        {
            return nextDriftPhase;
        }
        else return currentDriftPhase;
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (InputRouting.Instance.GetDriftInput() == false && !exitQueued) //canceling a drift input queues an exit
        {                                                                  
            exitQueued = true;
        }
        
        if (exitQueued && !currentlyRotating)
        {
            stateMachine.SwitchState(player.skatingState); // will run the exit code
        }
    }
    public void Drift()
    {
        DriftTurnPlayer();
        DriftForce();
    }
    
    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine(DriftRotationY(player.playerModelTransform, 0, 
            player.playerData.playerModelRotationSpeed));
        player.StartCoroutine(DriftRotationY(player.transform, player.transform.localEulerAngles.y + rotationAmount * driftDirection,
            player.playerData.playerModelRotationSpeed, true));
        
        ActionEvents.AddToStylePoints?.Invoke(player.playerData.driftStylePoints);
        
    }
    
    /// <summary>
    /// Will rotate a transform's Y value to a target value over time. Coroutine will end when the rotation is complete.
    /// </summary>
    /// <param name="transformToRotate"></param>
    /// <param name="targetYValue"></param>
    /// <param name="rotationSpeed"></param>
    /// <param name="addForce"></param>
    /// <returns></returns>
    private IEnumerator DriftRotationY(Transform transformToRotate, float targetYValue, float rotationSpeed, bool addForce = false)
    {
        Quaternion startRotation = transformToRotate.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetYValue, 0);
        current = 0;
        currentlyRotating = true;
        
        while (current < 1)
        {
            current += Time.fixedDeltaTime * rotationSpeed;
            transformToRotate.localRotation = Quaternion.Lerp(startRotation, targetRotation, current);
            
            yield return null;
        }

        currentlyRotating = false;
        if (addForce)
        {
            player.rb.AddForce(transformToRotate.forward * driftSpeedBoost, ForceMode.Impulse);
        }
    }
    
    private float CalculateTurnSharpness()
    {
        float currentInputDirection = InputRouting.Instance.GetMoveInput().x;
        if (currentInputDirection < 0)
        {
            return 0;
        }
        
        float extraSharpness = player.playerData.inputExtraDriftTurnSharpness * InputRouting.Instance.GetMoveInput().x;

        return extraSharpness * driftDirection;

    }
    
    private void DriftTurnPlayer()
    {
        player.transform.Rotate(0, (player.playerData.baseDriftTurnSharpness + CalculateTurnSharpness()) * driftDirection * Time.fixedDeltaTime, 0, Space.Self);
    }
    
    private void DriftForce()
    {
        player.rb.AddForce(player.inputTurningTransform.forward * player.playerData.baseDriftForce, ForceMode.Acceleration);
    }
}
