using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BulletInfo : MonoBehaviour
{
    #region PARAMETERS
    public GameObject bulletObject;

    public float speed = 15;
    public float fireRate = 1;
    public float timeToLive = 5;

    [Tooltip("How innacurate is the projectile? When being fired it's projected path will shift by this many degrees")]
    public float deviationAccuracy = 0;

    [Tooltip("How far up or down the player's linear trajectory will the bullet aim?")]
    public float deviationLead = 0;

    #endregion
}
