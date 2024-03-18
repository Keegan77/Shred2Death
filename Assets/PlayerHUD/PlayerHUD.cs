using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    StatsWidget stats;

    private void Awake()
    {
        stats = transform.Find("StatsWidget").GetComponent<StatsWidget>();
    }
}
