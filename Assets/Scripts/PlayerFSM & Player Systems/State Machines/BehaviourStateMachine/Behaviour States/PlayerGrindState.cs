using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerGrindState : BehaviourState
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
        behaviourInputActions.Add(InputRouting.Instance.input.Player.Jump, new InputActionEvents { onPerformed = ctx => JumpOffRail()});
    }
    private SplineFollower sFollower;
    //private Coroutine lerpRigRoutine;
    
    public override void Enter()
    {
        base.Enter();
        player.particleManager.playerGrindSparks.Play();
        totalInputRotation = 0;
        player.GetComboHandler().SetPauseComboDrop(true);
        
        player.proceduralRigController.SetWeightToValueOverTime(player.proceduralRigController.legRig,
            0, .1f);
        //lerpRigRoutine = player.proceduralRigController.currentRigWeightRequest;
        
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
        //if (lerpRigRoutine != null) player.StopCoroutine(lerpRigRoutine);
        player.particleManager.playerGrindSparks.Stop();
        player.GetComboHandler().SetPauseComboDrop(false);
        ActionEvents.StopLoopAudio?.Invoke();
        player.proceduralRigController.SetWeightToValueOverTime(player.proceduralRigController.legRig, 1, .1f);
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

        
        SetBaseFollowSpeed();

        // calculates the dot product of the player's velocity and the spline sample forward to determine if the player is moving forward or backward
        float dotProduct = Vector3.Dot(playerForward, splineTangent);
        
        sFollower.SetPercent(player.GetSplineCompletionPercent()); // moves the spline follower to be where the player jumped on
        
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
            jumpOffEndOfRailCoroutine = player.StartCoroutine(QueueEndRailDismount());
        }
    }

    private IEnumerator QueueEndRailDismount()
    {
        if (sFollower.direction == Spline.Direction.Forward)
        {
            yield return new WaitUntil(() => sFollower.result.percent == 1);
            JumpOffRail();
        }
        else
        {
            yield return new WaitUntil(() => sFollower.result.percent == 0);
            JumpOffRail();
        }
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
    private float totalInputRotation = 0f;
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (lerping) return;
        ModifyFollowSpeed();
        
        player.transform.position = sFollower.result.position + new Vector3(0, grindPosOffset, 0);
        // Calculate the grind turn
        float grindTurn = player.playerData.grindTurnSharpness * InputRouting.Instance.GetMoveInput().x * Time.fixedDeltaTime;

        // Accumulate the grind turn into totalInputRotation
        totalInputRotation += grindTurn;

        // Get the rotation from the grind rail and add the rotation when the player jumped on the rail
        float railRotation = sFollower.result.rotation.eulerAngles.y + jumpedOnOrientation.eulerAngles.y;

        // Add the totalInputRotation to the rail rotation
        float totalRotation = railRotation + totalInputRotation;

        // Apply the total rotation to the player
        player.transform.rotation = Quaternion.Euler(0, totalRotation, 0);

    }

    //deprecated
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

        if (player.grindSpeedOverride)
        {
            sFollower.followSpeed = player.overrideSpeed;
            currentSpeed = player.overrideSpeed;
            baseSpeed = player.overrideSpeed;
            player.grindSpeedOverride = false;
            return;
        }
        sFollower.followSpeed = currentSpeed;
    }
    
    private void ModifyFollowSpeed()
    {
        currentSpeed = baseSpeed + (player.playerData.grindSpeedAdditive * InputRouting.Instance.GetMoveInput().y);
        sFollower.followSpeed = currentSpeed * (sFollower.direction == Spline.Direction.Forward ? 1 : -1);
    }
    
}