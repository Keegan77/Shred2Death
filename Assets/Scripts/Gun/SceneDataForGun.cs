using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataForGun : MonoBehaviour
{
    [SerializeField] private GunData associatedGun;
    [SerializeField] private GameObject[] gunModels;
    [SerializeField] private Transform[] gunTips;
    [SerializeField] private Transform[] allTargets;
    [SerializeField] private Recoil[] recoilObjects;
    
    public GunData GetAssociatedGun()
    {
        return associatedGun;
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
    
    public Recoil[] GetRecoilObjects()
    {
        return recoilObjects;
    }
    
}
