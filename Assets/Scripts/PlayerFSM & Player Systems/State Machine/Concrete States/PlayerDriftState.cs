using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriftState : PlayerState
{
    // Start is called before the first frame update
    public PlayerDriftState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    private Vector3 cachedRotation;
    private float current, target;
    private float driftDirection; // -1 is left, 1 is right, 0 means no drift occurs
    public override void Enter()
    {
        base.Enter();
        driftDirection = Mathf.Sign(InputRouting.Instance.GetMoveInput().x);
        cachedRotation = player.playerModelTransform.localEulerAngles;
        current = 0;
        target = 1;
        player.StartCoroutine(LerpDriftingTransform());
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Drift();
        player.OrientToSlope();
        player.DeAccelerate();
        
        if (InputRouting.Instance.GetDriftInput() == false)
        {
            //stateMachine.SwitchState(player.skatingState); // will run the exit code
        }

    }
    
    public void Drift()
    {
        DriftTurnPlayer();
        DriftForce();
        //LerpDriftingTransform();
    }
    
    public override void Exit()
    {
        base.Exit();
        //player.inputTurningTransform.rotation = player.playerModelTransform.rotation;
        //player.playerModelTransform.localEulerAngles = cachedRotation;
    }
    
    private IEnumerator LerpDriftingTransform()
    {
        current = Mathf.MoveTowards(current, target, Time.fixedDeltaTime * player.playerData.playerModelRotationSpeed);
        while (current != 1)
        {
            player.playerModelTransform.localEulerAngles = Vector3.Lerp(player.playerModelTransform.localEulerAngles, new Vector3(0, 90, 0), current);
            yield return null;
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
