using System.Collections;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    private GameObject followerObj;
    private bool lerping;
    private float baseSpeed;
    private float currentSpeed;
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
        bool isClosed = false;
        followerObj = GameObject.Instantiate(player.grindRailFollower);
        sFollower = followerObj.GetComponent<SplineFollower>();
        sFollower.spline = player.GetCurrentSpline();
        sFollower.enabled = true;
        if (sFollower.spline.isClosed)
        {
            sFollower.wrapMode = SplineFollower.Wrap.Loop;
            isClosed = true;
        }
        else sFollower.wrapMode = SplineFollower.Wrap.Default;
        
        Vector3 playerForward = player.transform.forward;
        
        SplineSample sample = sFollower.spline.Project(player.transform.position);
        
        Vector3 splineTangent = sample.forward;

        
        if (player.rb.velocity.magnitude < 15) sFollower.followSpeed = 15;
        SetBaseFollowSpeed();

        // calculates the dot product of the player's velocity and the spline sample forward to determine if the player is moving forward or backward
        float dotProduct = Vector3.Dot(playerForward, splineTangent);
        Debug.Log($"Dot Product: {dotProduct}");

        // Set the SplineFollower to move forward or backward based on the dot product
        

        
        sFollower.SetPercent(player.GetSplineCompletionPercent());
        
        if (dotProduct > 0) // if the product is positive, that means we are moving in the direction of the spline
        {
            sFollower.direction = Spline.Direction.Forward;
            sFollower.followSpeed = Mathf.Abs(sFollower.followSpeed);
        }
        else
        {
            sFollower.direction = Spline.Direction.Backward;
            sFollower.followSpeed = -Mathf.Abs(sFollower.followSpeed);
            
        }

        if (!isClosed)
        {
            player.StartCoroutine(JumpOffEndOfRail());
        }
    }

    private IEnumerator JumpOffEndOfRail()
    {
        yield return new WaitUntil(() => sFollower.result.percent == 1 || sFollower.result.percent == 0);
        JumpOffRail();
    }
    
    private void JumpOffRail()
    {
        player.SetRBKinematic(false);
        //player.inputTurningTransform.rotation = player.transform.rotation;
        player.rb.AddForce(player.transform.up * player.playerData.baseJumpForce, ForceMode.Impulse);
        player.rb.AddForce(player.transform.forward * player.playerData.baseJumpForce, ForceMode.Impulse);
        //player.OllieJump();
        stateMachine.SwitchState(player.airborneState);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (lerping) return;
        ModifyFollowSpeed();

        player.transform.position = sFollower.result.position + new Vector3(0, player.playerData.grindPositioningOffset, 0);

        // Calculate the difference in rotation
        float rotationDifference = sFollower.result.rotation.eulerAngles.y - player.transform.rotation.eulerAngles.y;

        // Get the rotation from the player's input
        float playerInputRotation = player.playerData.grindTurnSharpness * InputRouting.Instance.GetMoveInput().x * Time.deltaTime;

        // Only rotate the player based on the player's input when the player is moving
        if (playerInputRotation != 0)
        {
            player.GetMovementMethods().TurnPlayer(true, playerInputRotation);
        }
        else
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.Euler(0, player.transform.eulerAngles.y + rotationDifference, 0);

            // Interpolate the player's rotation towards the target rotation over time
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.deltaTime * 30);
        }
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
    
    private void SetBaseFollowSpeed()
    {
        baseSpeed = Mathf.Clamp(player.rb.velocity.magnitude, player.playerData.minGrindSpeed, player.playerData.maxGrindSpeed);
        currentSpeed = baseSpeed;
        sFollower.followSpeed = currentSpeed;
    }
    
    private void ModifyFollowSpeed()
    {
        currentSpeed = baseSpeed + (player.playerData.grindSpeedAdditive * InputRouting.Instance.GetMoveInput().y);
        sFollower.followSpeed = currentSpeed * (sFollower.direction == Spline.Direction.Forward ? 1 : -1);
    }
    
}
