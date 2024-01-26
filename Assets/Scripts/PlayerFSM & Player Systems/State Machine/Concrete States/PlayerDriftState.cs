using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriftState : PlayerState
{
    // Start is called before the first frame update
    public PlayerDriftState(PlayerBase player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }
}
