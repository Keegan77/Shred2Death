using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomTrickMethods
{
    public delegate void TrickMethod(PlayerBase player);

    internal static void GeneralTrickFunc(PlayerBase player)
    {
        Debug.Log("A pop shuv it has been completed! This is custom code which is running on this trick alone!");
        ActionEvents.PlayerSFXOneShot?.Invoke(SFXContainerSingleton.Instance.popShuvItSound, 0);
    }

    internal static void OllieFunc(PlayerBase player)
    {
        List<AudioClip> ollieSounds = SFXContainerSingleton.Instance.ollieSounds;
        ActionEvents.PlayerSFXOneShot?.Invoke(ollieSounds[Random.Range(0, ollieSounds.Count)], 0);
    }
}
