using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy Animations sometimes have events on them.
/// Scripts that inherit off of this 
/// </summary>
public class Enemy_AnimEventHandler : MonoBehaviour
{
    public void OnFireball ()
    {
        Debug.Log ("FOIREBOWL");
    }
}
