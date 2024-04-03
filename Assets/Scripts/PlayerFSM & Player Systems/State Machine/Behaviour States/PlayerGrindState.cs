using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerGrindState : PlayerState
{
    private GameObject followerObj;
    private bool lerping;
    private float baseSpeed;
    private float currentSpeed;
    private float grindPosOffset = .21f;
    private Coroutine jumpOffEndOfRailCoroutine;
    private Quaternion jumpedOnOrientation;
    public PlayerGrindState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        inputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents { onPerformed = ctx => JumpOffRail()});
    }
    private SplineFollower sFollower;
    
    public override void Enter()
    {
        base.Enter();
        player.particlePlayer.PlayParticle();
        player.GetComboHandler().SetPauseComboDrop(true);
        /*player.proceduralRigController.StartCoroutine(
            player.proceduralRigController.LerpWeightToValue
                                                 (player.proceduralRigController.legRig,
                                                     0,
                                                     .1f)
        );*/
        ActionEvents.OnPlayBehaviourAnimation?.Invoke("Grind");
        lerping = true;
        List<AudioClip> grindImpacts = SFXContainerSingleton.Instance.grindImpactNoises;
        List<AudioClip> grindLoopSounds = SFXContainerSingleton.Instance.grindSounds;
        ActionEvents.PlayerSFXOneShot?.Invoke(grindImpacts[Random.Range(0, grindImpacts.Count)], 0);
        ActionEvents.PlayLoopAudio?.Invoke(grindLoopSounds[Random.Range(0, grindLoopSounds.Count)]);
        UnsubscribeInputs();
        SubscribeInputs();
        SetUpSplineFollower();
        
        jumpedOnOrientation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y - sFollower.result.rotation.eulerAngles.y, 0);
        player.transform.rotation = Quaternion.Euler(0, sFollower.result.rotation.eulerAngles.y + jumpedOnOrientation.eulerAngles.y, 0);
        player.StartCoroutine(LerpToFollower(player.playerData.railSnapTime));
        player.SetRBKinematic(true);
        
    }
    public override void Exit()
    {
        UnsubscribeInputs();
        player.particlePlayer.StopParticle();
        player.GetComboHandler().SetPauseComboDrop(false);
        ActionEvents.StopLoopAudio?.Invoke();
        player.proceduralRigController.StartCoroutine(
            player.proceduralRigController.LerpWeightToValue
            (player.proceduralRigController.legRig,
                1,
                .1f)
        );
        ActionEvents.OnPlayBehaviourAnimation?.Invoke("Grind Reverse");
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
        
        Vector3 playerForward = player.inputTurningTransform.forward;
        
        SplineSample sample = sFollower.spline.Project(player.transform.position);
        
        Vector3 splineTangent = sample.forward;

        
        if (player.rb.velocity.magnitude < 15) sFollower.followSpeed = 15;
        SetBaseFollowSpeed();

        // calculates the dot product of the player's velocity and the spline sample forward to determine if the player is moving forward or backward
        float dotProduct = Vector3.Dot(playerForward, splineTangent);

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
            jumpOffEndOfRailCoroutine = player.StartCoroutine(JumpOffEndOfRail());
        }
    }

    private IEnumerator JumpOffEndOfRail()
    {
        yield return new WaitUntil(() => sFollower.result.percent == 1 || sFollower.result.percent == 0);
        JumpOffRail();
    }
    
    private void JumpOffRail()
    {
        if (jumpOffEndOfRailCoroutine != null) player.StopCoroutine(jumpOffEndOfRailCoroutine);
        GameObject.Destroy(followerObj);
        player.transform.rotation = player.inputTurningTransform.rotation;
        player.inputTurningTransform.rotation = player.transform.rotation;
        player.SetRBKinematic(false);
        player.rb.velocity = Vector3.zero;
        player.rb.AddForce(player.transform.up * player.playerData.baseJumpForce, ForceMode.Impulse);
        player.rb.AddForce(player.inputTurningTransform.forward * player.playerData.baseJumpForce, ForceMode.Impulse);
        
        
        //player.OllieJump();
        stateMachine.SwitchState(player.airborneState);
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (lerping) return;
        ModifyFollowSpeed();
        
        player.transform.position = sFollower.result.position + new Vector3(0, grindPosOffset, 0);
        //player.transform.rotation = Quaternion.Euler(0, sFollower.result.rotation.eulerAngles.y + jumpedOnOrientation.eulerAngles.y, 0);
        GrindTurn(player.playerData.grindTurnSharpness);
        
    }

    private void GrindTurn(float turnSharpness)
    {
        player.transform.Rotate(0,
            turnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime, 
            0, Space.Self);
    }

    private IEnumerator LerpToFollower(float seconds)
    {
        float t = 0;
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        
        while (t < 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime * seconds);
            Vector3 endPos = sFollower.result.position + new Vector3(0, grindPosOffset, 0);
            Quaternion endRot = Quaternion.Euler(0, sFollower.result.rotation.eulerAngles.y + jumpedOnOrientation.eulerAngles.y, 0);
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