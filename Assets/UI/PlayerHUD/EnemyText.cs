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
    TMP_Text text;

    private void Awake ()
    {
        text = GetComponent<TMP_Text>();
    }

    public void setTextCount (int remEnemies)
    {
        text.text = $"{Mathf.Clamp (remEnemies, 0, 99)}";
    }
}
