using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// States are the primary system to
/// </summary>
public class Enemy_State : MonoBehaviour
{
    public static GameObject playerObject;

    //States will control movement directly.
    //Rigidbody will be set in the start function
    protected Enemy e;

    #region STATE MACHINE
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy>();
    }

    public virtual void Enter ()
    {

    }

    public virtual void Exit ()
    {

    }

    public virtual void machineUpdate ()
    {

    }

    public virtual void machinePhysics ()
    {

    }

    public virtual void onPlayerSensorActivated ()
    {

    }

    public virtual void onPlayerSensorDeactivated ()
    {

    }

    #endregion

    #region NAVIGATION
    public void EndPath ()
    {
        e.agentPath.ClearCorners ();
    }
    #endregion
}
