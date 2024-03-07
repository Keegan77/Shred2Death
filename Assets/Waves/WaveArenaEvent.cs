using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Wave events are modules that are added to WaveManager
/// Waves have their own opening and closing events
/// </summary>
/// 

[Serializable]
public class WaveArenaEvent
{
    public float eventTime = 0;
    public UnityEvent arenaEvents;
}
