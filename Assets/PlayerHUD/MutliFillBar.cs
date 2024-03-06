using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MutliFillBar : MonoBehaviour
{
    #region Fill Bars
    [Header("Values")]
    public float minValue;
    public float maxValue;

    [Min(1)]
    public int fillBarCount = 1;

    [Header("Inner Fills")]
    public Texture fillImage;
    #endregion

    #region CONSTANTS

    const string PATH_FILLBAR = "FillBars";
    #endregion

    #region STARTUP

    private void Start ()
    {
        
    }
    #endregion

    #region FUNCTIONS

    #endregion
}
