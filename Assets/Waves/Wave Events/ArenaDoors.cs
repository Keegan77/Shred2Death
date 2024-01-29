using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaDoors : Wave_Event
{
    public GameObject[] Doors;
    public override void event_Open ()
    {
        foreach (GameObject d in Doors)
        {
            d.SetActive (true);
        }
    }

    public override void event_Close ()
    {
        foreach (GameObject d in Doors)
        {
            d.SetActive (false);
        }
    }
}
