using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : MonoBehaviour
{
    public RawImage crosshair_Dot;
    public RawImage crosshair_Dualies;
    public RawImage crosshair_Shotgun;

    public void SetCrosshairActive(RawImage crossHair)
    {
        crosshair_Dualies.gameObject.SetActive(false);
        crosshair_Shotgun.gameObject.SetActive(false);
        
        crossHair.gameObject.SetActive(true);
    }
}
