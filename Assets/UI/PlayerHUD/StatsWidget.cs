using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Acts as an interface to the playerHUD widget to make accessing its information easier.
/// </summary>
public class StatsWidget : MonoBehaviour
{
    [Tooltip("Update the meterCurrentValue directly. The bar will fill on its own")]
    public MultiFillBar styleMeter;
    
    [Tooltip("Update the currentValue of this bar. The bar will fill on its own")]
    public FillBar boostMeter;

    [Tooltip("Use setTextCount() to update the text")]
    public EnemyText enemyText;

    public FillBar ammoBar;
    public TextMeshProUGUI ammoText;
    
}
