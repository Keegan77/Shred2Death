using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Empty : Enemy_State
{
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy>();
    }

    public override void AIUpdate ()
    {

    }
}
