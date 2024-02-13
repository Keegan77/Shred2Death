using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunfireHandler : MonoBehaviour
{
    private GunData currentGun;

    private float timeSinceLastShot;

    private bool reloading;

    private Transform castPoint;
    
    private bool CanShoot() =>
        !reloading && timeSinceLastShot > currentGun.timeBetweenShots; // if we're done firing the last shot

}
