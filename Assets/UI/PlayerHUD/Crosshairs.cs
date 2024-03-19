using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : MonoBehaviour
{
    public RawImage crosshair_Dot;
    public RawImage crosshair_Dualies;
    public RawImage crosshair_Shotgun;

    public void SetCrossHair(Image c)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        c.gameObject.SetActive(true);
    }
}
