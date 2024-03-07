using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave_EventedObject acts as an interface.
/// Put it on an object in the scene, to give it behaviors that can be caused by Arena Events.
/// 
/// Objects that inherit from this are abbreviated WEO_
/// </summary>
public class WaveEventedObject : MonoBehaviour
{
    //When placed in the opening events, do this
    public virtual void PlayEventOpen ()
    {
        Debug.Log ("Opening Event");
    }

    //When placed in the closing events, do this
    public virtual void PlayEventClose ()
    {
        Debug.Log ("Closing Event");
    }
}
