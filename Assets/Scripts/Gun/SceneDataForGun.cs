using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataForGun : MonoBehaviour
{
    [SerializeField] private GunData associatedGun;
    [SerializeField] private GameObject[] gunModels;
    [SerializeField] private Transform[] gunTips;
    [SerializeField] private Transform[] allTargets;
    [SerializeField] private Transform[] abilityTargets;
    [SerializeField] private Recoil[] recoilObjects;
    [SerializeField] private RawImage crosshair;
    
    public GunData GetAssociatedGun()
    {
        return associatedGun;
    }
    
    public RawImage GetCrosshair()
    {
        return crosshair;
    }
    
    public Transform[] GetGunTips()
    {
        return gunTips;
    }
    
    public GameObject[] GetGunModels()
    {
        return gunModels;
    }
    
    public Transform[] GetAllTargets()
    {
        return allTargets;
    }
    
    public Transform[] GetAbilityTargets()
    {
        return abilityTargets;
    }
    
    public Recoil[] GetRecoilObjects()
    {
        return recoilObjects;
    }
    
}
