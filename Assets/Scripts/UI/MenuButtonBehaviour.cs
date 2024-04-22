using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonBehaviour : MonoBehaviour
{
    private BounceUI bounceUI;
    

    public float restingPos;
    public float selectedPos;
    private bool disableHover;
    
    private void Start()
    {
        bounceUI = GetComponent<BounceUI>();
        bounceUI.targetXPosition = restingPos;
        disableHover = false;
    }
    
    public void ChangeTargetPosition(float xPosition)
    {
        if (disableHover) return;
        bounceUI.targetXPosition = xPosition;
    }
    
    public void DisableHover()
    {
        disableHover = true;
    }
}
