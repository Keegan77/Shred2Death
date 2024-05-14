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
        gameObject.SetActive(true);
        defaultButtonActive.Select();
        
        // Get all BounceUI components attached to this object and its children
        BounceUI[] bounceUIComponents = GetComponentsInChildren<BounceUI>();

        // Call MoveToEndValue on each BounceUI component
        foreach (BounceUI bounceUI in bounceUIComponents)
        {
            bounceUI.MoveToEndValue();
        }
    }
}
