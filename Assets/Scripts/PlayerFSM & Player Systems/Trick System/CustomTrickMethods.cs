using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomTrickMethods
{
    public delegate void TrickMethod(PlayerBase player);

    internal static void PopShuvItCustomFunction(PlayerBase player)
    {
        Debug.Log("A pop shuv it has been completed! This is custom code which is running on this trick alone!");
        player.GetMovementMethods().OllieJump();
    }
}
