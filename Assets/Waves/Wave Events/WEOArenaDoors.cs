using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WEOArenaDoors : WaveEventedObject
{
    public override void PlayEventOpen ()
    {
        gameObject.SetActive (true);
    }

    public override void PlayEventClose ()
    {
        gameObject.SetActive (false);
    }
}
