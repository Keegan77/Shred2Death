using System.Collections;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    private GameObject followerObj;
    private bool lerping;
    public PlayerGrindState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents { onPerformed = ctx => JumpOffRail()});
    }
    private SplineFollower sFollower;
    
    public override void Enter()
    {
        base.Enter();
        lerping = true;
        SubscribeInputs();
        SetUpSplineFollower();
        player.StartCoroutine(LerpToFollower(player.playerData.railSnapTime));
        player.SetRBKinematic(true);
        
    }
    public override void Exit()
    {
        UnsubscribeInputs();
        GameObject.Destroy(followerObj);
    }
    
    
    private void SetUpSplineFollower()
    {
        followerObj = GameObject.Instantiate(player.grindRailFollower);
        sFollower = followerObj.GetComponent<SplineFollower>();
        sFollower.spline = player.GetCurrentSpline();
        sFollower.enabled = true;
        
        Vector3 playerForward = player.inputTurningTransform.forward;
        
        SplineSample sample = sFollower.spline.Project(player.transform.position);
        
        Vector3 splineTangent = sample.forward;

        
        if (player.rb.velocity.magnitude < 15) sFollower.followSpeed = 15;
        else sFollower.followSpeed = player.rb.velocity.magnitude;

        // calculates the dot product of the player's velocity and the spline sample forward to determine if the player is moving forward or backward
        float dotProduct = Vector3.Dot(playerForward, splineTangent);

        // Set the SplineFollower to move forward or backward based on the dot product
        
        if (dotProduct > 0) // if the product is positive, that means we are moving in the direction of the spline
        {
            sFollower.followSpeed = Mathf.Abs(sFollower.followSpeed);
            sFollower.direction = Spline.Direction.Forward;
        }
        else
        {
            sFollower.followSpeed = -Mathf.Abs(sFollower.followSpeed);
            sFollower.direction = Spline.Direction.Backward;
        }
        
        sFollower.SetPercent(player.GetSplineCompletionPercent());
    }

    private void JumpOffRail()
    {
        player.SetRBKinematic(false);
        player.rb.AddForce(player.transform.up * player.playerData.baseJumpForce, ForceMode.Impulse);
        player.rb.AddForce(player.inputTurningTransform.forward * player.playerData.baseJumpForce, ForceMode.Impulse);
        //player.OllieJump();
        stateMachine.SwitchState(player.airborneState);
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (lerping) return;
        
        player.transform.position = sFollower.result.position + new Vector3(0, player.playerData.grindPositioningOffset, 0);
        player.transform.localEulerAngles = new Vector3(0, sFollower.result.rotation.eulerAngles.y, 0);
        player.TurnPlayer(true, player.playerData.grindTurnSharpness * Time.deltaTime);
        
    }

    private IEnumerator LerpToFollower(float seconds)
    {
        float t = 0;
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        
        while (t < 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime * seconds);
            Vector3 endPos = sFollower.result.position + new Vector3(0, player.playerData.grindPositioningOffset, 0);
            Quaternion endRot = new Quaternion(0, sFollower.result.rotation.y, 0, sFollower.result.rotation.w);
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            player.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        lerping = false;
    }

    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Calculate the new position
    }
}
