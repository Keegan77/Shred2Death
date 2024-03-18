using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Tooltip("See the tooltips on the StatsWidget object to see how to use the widget.")]
    StatsWidget stats;

    private void Awake()
    {
        stats = transform.Find("StatsWidget").GetComponent<StatsWidget>();
    }
}
