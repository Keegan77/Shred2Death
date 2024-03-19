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
    [Tooltip("In seconds, the amount of time the bullet takes to get to it's end destination.")]
    [Range(0.0001f, .5f)]
    public float bulletLerpTime;
    [Tooltip("Amount of bullets that will be fired per mouse click")]
    public int bulletsInOneShot;


    [Header("Reloading")]
    public int magCapacity;
    public float timeBetweenShots;

    
    [Header("Camera Recoil")]
    public float camRecoilX;
    public float camRecoilY;
    public float camRecoilZ;
    
    [Header("Gun Recoil")]
    public float gunRecoilX;
    public float gunRecoilY;
    public float gunRecoilZ;
    

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

    [Header("Audio")] public List<AudioClip> fireSounds;

    public List<float> delayPerAudioClip;
}
