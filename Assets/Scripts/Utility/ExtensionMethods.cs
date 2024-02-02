using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsInRangeOf(this float value, float min, float max)
    {
        return value >= min && value <= max;
    }
}
