using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave events are modules that are added to WaveManager
/// Waves have their own opening and closing events
/// </summary>
/// 

[Serializable]
public class Wave_Event : MonoBehaviour
{
    public float eventTime = 0;
    //When placed in the opening events, do this
    public virtual void event_Open ()
    {
        Debug.Log ("Opening Event");
    }

    //When placed in the closing events, do this
    public virtual void event_Close ()
    {
        Debug.Log ("Closing Event");
    }

    public IEnumerator eventDelay ()
    {
        yield return new WaitForSeconds (eventTime);
    }
}
