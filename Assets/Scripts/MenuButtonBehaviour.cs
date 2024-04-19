using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonBehaviour : MonoBehaviour
{
    private BounceUI bounceUI;
    

    public float restingPos;
    public float selectedPos;
    
    private void Start()
    {
        bounceUI = GetComponent<BounceUI>();
        bounceUI.targetXPosition = restingPos;
    }
    
    public void ChangeTargetPosition(float xPosition)
    {
        bounceUI.targetXPosition = xPosition;
    }
}
