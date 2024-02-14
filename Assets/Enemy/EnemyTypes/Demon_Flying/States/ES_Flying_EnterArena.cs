using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Flying_EnterArena : ES_Type_Flying
{
    public override void Enter ()
    {
        StartCoroutine (MoveToPoint (e.stateMachine.travelPoint));

    }
}
