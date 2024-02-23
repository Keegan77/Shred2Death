using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// When activated, flying enemies will chase the player
/// </summary>
public class ES_Flying_Chase : EState_Flying
{
    public override void Enter ()
    {
        StartCoroutine (MoveToObject (Enemy.playerObject));
    }

    public override void machinePhysics ()
    {
        base.machinePhysics ();
    }
    protected override void onPointReached ()
    {
        base.onPointReached ();
        Debug.Log ($"{name}: Reached point and will melee the player now.");
    }
}
