using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunfireHandler))]
public class GunSwitcher : MonoBehaviour
{
    GunfireHandler gunfireHandler;
    [SerializeField] List<GunData> guns;
    [SerializeField] SceneDataForGun[] sceneDataForGuns;
    private GunSwitchData switchData;
    [SerializeField] private GameObject rigTargetsParent;
    FollowTargetProperties[] rigTargets;
    private bool gunSwitchQueued;
    
    private void Start()
    {
        rigTargets = rigTargetsParent.GetComponentsInChildren<FollowTargetProperties>();
        gunfireHandler = GetComponent<GunfireHandler>();
        switchData = new GunSwitchData(gunfireHandler.GetCurrentGunData(), gunfireHandler.GetCurrentGunSceneData()); // move from here, just a syntax reminder
        SetModels(GetNextGunSceneData().GetGunModels(), false);
        SetRigTargetPoints(gunfireHandler.GetCurrentGunSceneData().GetAllTargets());
        
        foreach (var gun in guns)
        {
            gun.currentAmmo = gun.magCapacity;
        }
    }

    private void OnEnable()
    {
        InputRouting.Instance.input.Player.SwitchGun.performed += ctx =>
        {
            StartCoroutine(QueueGunSwitch());
        };
        ActionEvents.OnGunSwitch += HandleOnGunSwitch;
    }
    private void OnDisable()
    {
        InputRouting.Instance.input.Player.SwitchGun.performed -= ctx => StartCoroutine(QueueGunSwitch());
        ActionEvents.OnGunSwitch -= HandleOnGunSwitch;
    }

    private GunData GetNextGun()
    {
        foreach (var gun in guns)
        {
            if (gun != gunfireHandler.GetCurrentGunData())
            {
                return gun;
            }
        }
        return null;
    }
    private SceneDataForGun GetNextGunSceneData()
    {
        foreach (var sceneData in sceneDataForGuns)
        {
            if (gunfireHandler.GetCurrentGunSceneData() != sceneData)
            {
                return sceneData;
            }
        }
        return null;
    }
    
    private SceneDataForGun GetCurrentGunSceneData(GunData gun)
    {
        foreach (var sceneData in sceneDataForGuns)
        {
            if (gun == sceneData.GetAssociatedGun())
            {
                return sceneData;
            }
        }
        return null;
    }
    
    private IEnumerator QueueGunSwitch()
    {
        if (gunSwitchQueued) yield break;
        
        gunSwitchQueued = true;
        
        yield return new WaitUntil(() => gunfireHandler.CanShoot());
        // set switch data

        SetModels(switchData.SceneDataForGun.GetGunModels(), false);
        switchData.GunData = GetNextGun();
        switchData.SceneDataForGun = GetCurrentGunSceneData(switchData.GunData);
        SetModels(switchData.SceneDataForGun.GetGunModels(), true);
        
        
        ActionEvents.OnGunSwitch?.Invoke(switchData);
        gunSwitchQueued = false;
    }

    private void SetModels(GameObject[] models, bool activeStatus)
    {

        foreach (var model in models)
        {
            model.SetActive(activeStatus);
            if (activeStatus == true) StartCoroutine(ScaleUpFromZero(model, .2f));
        }
    }
    
    private IEnumerator ScaleUpFromZero(GameObject model, float lerpTime)
    {
        float timeElapsed = 0;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = model.transform.localScale;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        while (timeElapsed < lerpTime)
        {
            timeElapsed += Time.deltaTime;
            model.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(timeElapsed / lerpTime));
            yield return null;
        }
    }
    
    private void HandleOnGunSwitch(GunSwitchData switchData)
    {
        Transform[] transformTargets = switchData.SceneDataForGun.GetAllTargets();
        
        SetRigTargetPoints(transformTargets);
        gunfireHandler.SetCurrentGun(switchData);
    }
    private void SetRigTargetPoints(Transform[] transformTargets)
    {
        foreach (var target in rigTargets)
        {
            target.SetOriginPoint(GetTargetByTag(transformTargets, target.gameObject.tag));
        }
    }
    
    private Transform GetTargetByTag(Transform[] transformTargets, string tag)
    {
        foreach (var target in transformTargets)
        {
            if (target.gameObject.CompareTag(tag))
            {
                return target;
            }
        }

        return null;
    }
}
