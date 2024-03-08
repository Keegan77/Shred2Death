using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
<<<<<<<< Updated upstream:Assets/PlayerHUD/MultiFillBar/MutliFillBar.cs
/// 
public class MutliFillBar : MonoBehaviour
========
public class MultiFillBar : MonoBehaviour
>>>>>>>> Stashed changes:Assets/PlayerHUD/MultiFillBar.cs
{
    #region Fill Bars
    [Header("Parameters")]
    public float meterMaxValue = 100;

    [Tooltip("Offset for the fillbars at 0% meter")]
    [Range(0f, 1f)]
    [SerializeField] float fillOffsetMin = 0;

    [Tooltip("Offset for the fillbars at 100% meter")]
    [Range(0f, 1f)]
    [SerializeField] float fillOffsetMax = 1;

    [Header ("Values")]
    public float meterCurrentValue;

    #endregion

    #region private
    Transform fillbarContainer;
    int fillbarCount;
    float fillbarMaxValue;
    #endregion

    #region CONSTANTS
    const string PATH_FILLBAR = "FillBars";
    #endregion


    private void Start ()
    {
        meterCurrentValue = 0;

        fillbarContainer = transform.Find(PATH_FILLBAR);

        fillbarCount = fillbarContainer.childCount;
        fillbarMaxValue = meterMaxValue / fillbarCount;
    }



    private void Update ()
    {
        for (int i = 0 ; i < fillbarCount ; i++) 
        {
            Image img = fillbarContainer.GetChild(i).GetComponent<Image>();

<<<<<<<< Updated upstream:Assets/PlayerHUD/MultiFillBar/MutliFillBar.cs
            float fillAmount = (meterCurrentValue - (i * (meterMaxValue / fillbarContainer.childCount))) / fillbarMaxValue;
========
            // Percentage X
            float fillAmount = (meterCurrentValue - (i * fillbarMaxValue)) / fillbarMaxValue;
>>>>>>>> Stashed changes:Assets/PlayerHUD/MultiFillBar.cs

            // What is X percent of the difference between offsetMin and Max? Offset this to the minimum fill
            fillAmount = fillAmount * (fillOffsetMax - fillOffsetMin) + fillOffsetMin;

            img.fillAmount = Mathf.Clamp (fillAmount, fillOffsetMin, fillOffsetMax);
        }
    }
}

