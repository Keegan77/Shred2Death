using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class MutliFillBar : MonoBehaviour
{
    #region Fill Bars
    [Header ("Values")]
    [Min(1)]
    public float meterMaxValue = 1;
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

            float fillAmount = (meterCurrentValue - (i * fillbarMaxValue)) / fillbarMaxValue;

            img.fillAmount = Mathf.Clamp (fillAmount, 0, 1);
        }
    }
}

