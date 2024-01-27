using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriftState : PlayerState
{
    // Start is called before the first frame update
    public PlayerDriftState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    private float rotationAmount;
    private bool currentlyRotating;
    private bool exitQueued;
    private float current, target;
    private float driftDirection; // -1 is left, 1 is right, 0 means no drift occurs
    public override void Enter()
    {
        base.Enter();
        rotationAmount = player.playerData.driftRotationalOffset;
        exitQueued = false;
        driftDirection = Mathf.Sign(InputRouting.Instance.GetMoveInput().x);
        current = 0;
        target = 1;
        player.StartCoroutine(DriftRotationY(player.playerModelTransform, driftDirection * rotationAmount, player.playerData.playerModelRotationSpeed));
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Drift();
        player.OrientToSlope();
        player.DeAccelerate();
        

    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (InputRouting.Instance.GetDriftInput() == false && !exitQueued)
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
        player.StartCoroutine(DriftRotationY(player.inputTurningTransform, player.inputTurningTransform.localEulerAngles.y + rotationAmount * driftDirection,
            player.playerData.playerModelRotationSpeed, true));
        
        
    }
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
            player.rb.AddForce(transformToRotate.forward * player.playerData.driftBoostAmount, ForceMode.Impulse);
        }
    }
    
    private void DriftTurnPlayer()
    {
        player.inputTurningTransform.Rotate(0, player.playerData.baseDriftTurnSharpness * driftDirection * Time.fixedDeltaTime, 0, Space.Self);
    }
    
    private void DriftForce()
    {
        player.rb.AddForce(player.inputTurningTransform.forward * (player.playerData.baseDriftForce), ForceMode.Acceleration);
    }
}
