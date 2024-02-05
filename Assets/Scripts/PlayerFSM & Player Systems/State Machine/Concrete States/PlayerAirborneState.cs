using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents { onPerformed = ctx => CheckForSpline()});
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
        
        player.GetOrientationHandler().ReOrient();
        player.GetMovementMethods().TurnPlayer();
        AddAirForce();
        
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
    
    private void CheckForSpline()
    {
        float radius = 10f;
        
        RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, radius, player.transform.forward, 0, 1 << LayerMask.NameToLayer("Spline"));
        
        
        foreach (RaycastHit hit in hits)
        {
            SplineComputer hitSpline = hit.collider.GetComponent<SplineComputer>();
            SplineSample hitPoint = hitSpline.Project(player.transform.position);
            //Debug.Log(Vector3.Distance(player.transform.position, hitPoint.position));
            if (Vector3.Distance(player.transform.position, hitPoint.position) < 4f)
            {
                player.SetCurrentSpline(hitSpline, hitPoint);
                stateMachine.SwitchState(player.grindState);
            }
        }
    }
    
    /*
    public override void StateCollisionEnter(Collision other)
    {
        base.StateCollisionEnter(other);
        if (other.gameObject.TryGetComponent<SplineComputer>(out SplineComputer hitSpline) && player.rb.velocity.y < 0)
        {
            // Project the player's position onto the spline
            SplineSample hitPoint = hitSpline.Project(player.transform.position);
            Debug.Log("Player position: " + player.transform.position);
            Debug.Log("Hit point: " + hitPoint.position);
            Debug.Log("Hit point percent: " + hitPoint.percent);
            player.SetCurrentSpline(hitSpline, hitPoint);
            stateMachine.SwitchState(player.grindState);
            
        }
    }*/
}
