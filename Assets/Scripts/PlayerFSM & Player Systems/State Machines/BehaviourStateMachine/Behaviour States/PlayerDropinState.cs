using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropinState : BehaviourState
{
    public PlayerDropinState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    
    RaycastHit bowlSurfaceHit;
    
    public void SetBowlSurfaceHit(RaycastHit hit)
    {
        bowlSurfaceHit = hit;
    }

    public override void Enter()
    {
        base.Enter();
        player.StartCoroutine(LerpToBowlSurface());
    }
    
    private IEnumerator LerpToBowlSurface()
    {
        float t = 0;

        // end goal rotation is the perpendicular rotation of the bowl surface
        Quaternion targetRotation = Quaternion.FromToRotation(player.transform.up, bowlSurfaceHit.normal)
                                    * player.transform.rotation;

        // Lerp to the desired rotation
        player.rb.AddRelativeForce(player.transform.up * player.playerData.baseJumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(.1f);

        // First while loop for controlling the rotation
        while (t < 1)
        {
            t += Time.deltaTime * 3;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, t);
            player.rb.velocity = Vector3.Lerp(player.rb.velocity, new Vector3(0, player.rb.velocity.y, 0), t);
            yield return null;
        }

        // Reset t for the position lerp
        t = 0;


        stateMachine.SwitchState(player.halfPipeState);
        Debug.Log("State switched");
    }
        
        
}


