using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropinState : PlayerState
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
        yield return new WaitForSeconds(.2f);

        // First while loop for controlling the rotation
        while (t < 1)
        {
            t += Time.deltaTime;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, t);
            yield return null;
        }

        // Reset t for the position lerp
        t = 0;

        // Second while loop for controlling the position
        /*while (t < 1)
        {
            Vector3 startPos = player.transform.position;
            Vector3 endPos = new Vector3(bowlSurfaceHit.point.x, player.transform.position.y, bowlSurfaceHit.point.z);
            t += Time.deltaTime;

            // Perform the lerp operation only on the X and Z coordinates
            player.transform.position = new Vector3(
                Mathf.Lerp(startPos.x, endPos.x, t),
                player.transform.position.y,
                Mathf.Lerp(startPos.z, endPos.z, t)
            );

            yield return null;
        }*/

        stateMachine.SwitchState(player.halfPipeState);
    }
        
        
}


