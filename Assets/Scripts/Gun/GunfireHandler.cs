using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GunfireHandler : MonoBehaviour
{
    private float timeSinceLastShot;
    [SerializeField] private GunData currentGun;
    [SerializeField] private SceneDataForGun currentGunSceneData;

    private bool buttonSet; // true if we've set a button listener (we have a non-automatic weapon)

    [SerializeField] private Recoil cameraRecoil;

    [SerializeField] private Transform castPoint;
    
    [SerializeField] private TrailRenderer bulletTrail;

    private Transform[] currentGunTips;
    private Recoil[] currentGunRecoilScripts;
    private Recoil currentGunRecoilScript;

    private Transform currentGunTip;

    private void Awake()
    {
        currentGunTips = currentGunSceneData.GetGunTips();
        currentGunRecoilScripts = currentGunSceneData.GetRecoilObjects();
        
        SetUpGun();
    }

    private void SetUpGun() // called on start and on gun switch
    {
        if (buttonSet)
        {
            DisableFireButtonListeners();
        }
        currentGunTip = currentGunTips[0];
        currentGunRecoilScript = currentGunRecoilScripts[0];
        bulletTrail.time = currentGun.bulletLerpTime;
        if (!currentGun.automatic)
        {
            SetFireButtonListener(); // if gun isn't auto then we just set gunfire to our button down input
            buttonSet = true;
        }
    }

    public bool CanShoot() =>
        timeSinceLastShot > currentGun.timeBetweenShots; // we don't check ammo here because it's checked
                                                                       // in the fire method   

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (currentGun.automatic && CanShoot() && InputRouting.Instance.GetFireHeld())
        {
            Fire();
        } // if the gun is automatic, and we can shoot, and we're holding the fire button, then fire
        
    }

    private void Fire()
    {
        RaycastHit hit; //instantiate our raycast ref
        TrailRenderer trail; // instantiate our gun trail
        
        if (currentGun.currentAmmo <= 0) return;
        
        cameraRecoil.FireRecoil(currentGun.camRecoilX, currentGun.camRecoilY, currentGun.camRecoilZ); // apply recoil
        currentGunRecoilScript.FireRecoil(currentGun.gunRecoilX, currentGun.gunRecoilY, currentGun.gunRecoilZ);
        
        for (int i = 0; i < currentGun.bulletsInOneShot; i++) 
        {
            Vector3 direction = GetDirection(); 
            if (Physics.Raycast(castPoint.position, direction, out hit, currentGun.maxDistance)) //if we hit an object with our bullet
            {
                trail = Instantiate(bulletTrail, currentGunTip.position, Quaternion.identity); //start a bullet trail effect

                StartCoroutine(SpawnBullet(trail, hit.point, currentGunTip.position, hit)); //spawn our bullet
            }
            else // if we shoot, but we don't hit anything (if we shoot into the air at no objects, 
                 //we still want to show our bullet trail)
            {
                trail = Instantiate(bulletTrail, currentGunTip.position, Quaternion.identity);

                StartCoroutine(SpawnBullet(trail, castPoint.position + direction * (currentGun.maxDistance), currentGunTip.position, hit)); // sets the point of where our raycast would have ended up if it hit anything (point in the air)
                                   
            }
        }
        currentGun.currentAmmo--;
        
        timeSinceLastShot = 0;
        
        if (currentGun.alternateFire)
        {
            currentGunTip = currentGunTip == currentGunTips[0] ? currentGunTips[1] : currentGunTips[0];
            currentGunRecoilScript = currentGunRecoilScript == currentGunRecoilScripts[0] ? currentGunRecoilScripts[1] : currentGunRecoilScripts[0];
        }
        
        
            
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpawnBullet(TrailRenderer trail, Vector3 hitPos, Vector3 gunTip, RaycastHit hit)
    {
        float time = 0; 
        while (time < 1)
        {
            Vector3 startPosition = gunTip;
            trail.transform.position = Vector3.Lerp(startPosition, hitPos, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hitPos;
        
        Destroy(trail.gameObject, trail.time);
        
        IDamageable damageable = hit.transform?.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(currentGun.damage);
        }
        
    }
    
    
    private Vector3 GetDirection()
    {
        Vector3 direction = castPoint.forward;

        direction += new Vector3(Random.Range(-currentGun.spreadX, currentGun.spreadX),
                                 Random.Range(-currentGun.spreadY, currentGun.spreadY),
                                 Random.Range(-currentGun.spreadZ, currentGun.spreadZ));
        direction.Normalize();
        return direction;
    }

    public GunData GetCurrentGunData()
    {
        return currentGun;
    }
    
    public SceneDataForGun GetCurrentGunSceneData()
    {
        return currentGunSceneData;
    }

    public void SetCurrentGun(GunSwitchData switchData) // subscribe to gun switch event
    {
        currentGun = switchData.GunData;
        currentGunSceneData = switchData.SceneDataForGun;
        
        currentGunTips = currentGunSceneData.GetGunTips();
        currentGunRecoilScripts = currentGunSceneData.GetRecoilObjects();

        SetUpGun();
    }

#region Input
    private void SetFireButtonListener()
    {
        InputRouting.Instance.input.Player.Fire.performed += ctx =>
        {
            if (CanShoot())
            {
                Fire();
            }
        };
    }
    private void DisableFireButtonListeners()
    {
        InputRouting.Instance.input.Player.Fire.performed -= ctx =>
        {
            if (CanShoot())
            {
                Fire();
            }
        };
    }

    private void IncreaseAmmo(Trick trick)
    {
        currentGun.currentAmmo += trick.ammoBonus;
        if (currentGun.currentAmmo > currentGun.magCapacity) currentGun.currentAmmo = currentGun.magCapacity;
    }

    private void OnEnable()
    {
        ActionEvents.OnTrickCompletion += IncreaseAmmo;
    }

    private void OnDisable()
    {
        if (buttonSet) DisableFireButtonListeners();
        ActionEvents.OnTrickCompletion -= IncreaseAmmo;
    }
#endregion
}
