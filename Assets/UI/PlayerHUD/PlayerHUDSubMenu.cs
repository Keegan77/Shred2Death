using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Submenus attached to the player that contain information pertaining to which button should be activated first.
/// </summary>
public class PlayerHUDSubMenu : MonoBehaviour
{
    public Button defaultButtonActive;

    public void OnActivated()
    {
        defaultButtonActive.Select();
    }
}
