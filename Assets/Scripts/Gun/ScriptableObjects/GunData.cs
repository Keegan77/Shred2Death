using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Gun", menuName ="Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Tooltip("Will be used for the gun model pertaining to each gun.")]
    public GameObject gunPrefab;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    public bool automatic;
    [Tooltip("In seconds, the amount of time the bullet takes to get to it's end destination.")]
    public float bulletLerpTime;
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
    [Tooltip("Speed of the recoil snapback")]
    public float snappiness;
    [Tooltip("Speed of the recoil return to 0, 0, 0.")]
    public float returnSpeed;

    [Header("Spread")]
    public float spreadX;
    public float spreadY;
    public float spreadZ;

    [Header("General")]
    [Tooltip("Seconds")]
    public float readyUpTime;
  

    [HideInInspector] public float currentAmmo; //stored on a per-gun basis to keep the value between gun switches

    [Tooltip("Setting this to true will alternate the firing of the gun between the player model's " +
             "left and right hand. Useful for duelies.")]
    [SerializeField] public bool alternateFire;
}
