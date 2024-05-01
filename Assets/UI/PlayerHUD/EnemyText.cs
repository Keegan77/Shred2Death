using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Provides a way to set the text and clamp it properly using an int value.
/// Will be called off the player by active WaveManagers in the level.
/// </summary>
public class EnemyText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] private bool disabled;

    private void Awake ()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (disabled)
        {
            text.text = "";
        }
    }

    public void setTextCount (int remEnemies)
    {
        if (disabled)
        {
            text.text = "";
            return;
        }
        text.text = $"{Mathf.Clamp (remEnemies, 0, 99)}";
    }
}
