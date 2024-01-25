using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// States are the primary system to
/// </summary>
public class Enemy_State : MonoBehaviour
{
    public static GameObject playerObject;
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

}
