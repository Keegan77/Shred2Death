using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : BehaviourState
{
    public PlayerDeathState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.GetRagdollHandler().ActivateRagdoll();
        player.StartCoroutine(DelayGameOver());
    }
    
    private IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(3);
        player.playerHUD.openMenu(player.playerHUD.menuGameOver);
    }
}
