using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Gun", menuName ="Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;


    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    public bool automatic;
    [Tooltip("Amount of bullets that will be fired per mouse click")]
    public int bulletsInOneShot;


    [Header("Reloading")]
    public int magCapacity;
    public float timeBetweenShots;
    public float reloadTime;

    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float snappiness;
    public float returnSpeed;

    [Header("Spread")]
    public float spreadX;
    public float spreadY;
    public float spreadZ;

    [Header("General")]
    [Tooltip("Seconds")]
    public float readyUpTime;
  

    [HideInInspector] public float currentAmmo;
}
