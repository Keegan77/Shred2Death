using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(GunfireHandler))]
public class GunSwitcher : MonoBehaviour
{
    GunfireHandler gunfireHandler;
    [SerializeField] List<GunData> guns;
    [SerializeField] SceneDataForGun[] sceneDataForGuns;
    private GunSwitchData switchData;
    [SerializeField] TwoBoneIKConstraint leftArmMover,rightArmMover;
    [SerializeField] RigBuilder rigBuilder;
    private bool gunSwitchQueued;
    
    private void Start()
    {
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
            model.GetComponent<MeshRenderer>().enabled = activeStatus;
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
        Transform leftHandTarget = GetTargetByTag(transformTargets, "LeftHandTarget");
        Transform leftHandHint = GetTargetByTag(transformTargets, "LeftHandHint");
        Transform rightHandTarget = GetTargetByTag(transformTargets, "RightHandTarget");
        Transform rightHandHint = GetTargetByTag(transformTargets, "RightHandHint");

        void SetProperties(Transform target, Transform newTarget)
        {
            target.position = newTarget.position;
            target.rotation = newTarget.rotation;
            target.parent = newTarget.parent;
        }

        IEnumerator LerpRigTargetPoints()
        {
            float timeElapsed = 0;
            float lerpTime = .5f;
            while (timeElapsed < lerpTime)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / lerpTime;
                
                //quick lambda function for lerping position and then rotation
                Action<Transform, Transform, float> LerpPositionAndRotation = (currentTransform, targetTransform, t) =>
                {
                    currentTransform.position = Vector3.Lerp(currentTransform.position,
                                                             targetTransform.position, t);
                    currentTransform.rotation = Quaternion.Lerp(currentTransform.rotation,
                                                                targetTransform.rotation, t);
                };
                LerpPositionAndRotation(leftArmMover.data.target, leftHandTarget, t);
                LerpPositionAndRotation(leftArmMover.data.hint, leftHandHint, t);
                LerpPositionAndRotation(rightArmMover.data.target, rightHandTarget, t);
                LerpPositionAndRotation(rightArmMover.data.hint, rightHandHint, t);
                yield return null;
            }
            SetProperties(leftArmMover.data.target, leftHandTarget);
            SetProperties(leftArmMover.data.hint, leftHandHint);
            SetProperties(rightArmMover.data.target, rightHandTarget);
            SetProperties(rightArmMover.data.hint, rightHandHint);
        }
        StartCoroutine(LerpRigTargetPoints());
        //lerp the target and hint to the new gun's target and hint, and then set the properties after the lerp is done (the lerp time should be a fixed value)
        
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
