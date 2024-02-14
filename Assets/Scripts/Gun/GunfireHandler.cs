using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GunfireHandler : MonoBehaviour
{
    [SerializeField] private GunData currentGun;

    private float timeSinceLastShot;

    private bool reloading;

    private bool buttonSet; // true if we've set a button listener (we have a non-automatic weapon)
    
    [SerializeField] private CameraRecoil recoilScript;

    [SerializeField] private Transform castPoint;
    
    [SerializeField] private TrailRenderer bulletTrail;

    [SerializeField] private Transform gunTip;
    
    [SerializeField] private Transform alternateGunTip;

    private Transform currentGunTip;

    private void Start()
    {
        SetUpGun();
        currentGun.currentAmmo = currentGun.magCapacity;
    }

    private void SetUpGun() // called on start and on gun switch
    {
        if (buttonSet)
        {
            DisableFireButtonListeners();
        }
        currentGunTip = gunTip;
        bulletTrail.time = currentGun.bulletLerpTime;
        if (!currentGun.automatic)
        {
            SetFireButtonListener(); // if gun isn't auto then we just set gunfire to our button down input
            buttonSet = true;
        }
    }

    private bool CanShoot() =>
        timeSinceLastShot > currentGun.timeBetweenShots && !reloading; // we don't check ammo here because it's checked
                                                                       // in the fire method   

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (currentGun.automatic && CanShoot() && InputRouting.Instance.GetFireHeld())
        {
            Fire();
        } // if the gun is automatic, and we can shoot, and we're holding the fire button, then fire
        
        Debug.Log(currentGun.currentAmmo);
    }



    private void Fire()
    {
        RaycastHit hit; //instantiate our raycast ref
        TrailRenderer trail; // instantiate our gun trail

        if (reloading) return;
        if (currentGun.currentAmmo <= 0)
        {
            StartCoroutine(ReloadGun());
            return;
        }
        
        recoilScript.FireRecoil(); 
        
        for (int i = 0; i < currentGun.bulletsInOneShot; i++) 
        {
            Vector3 direction = GetDirection(); 
            if (Physics.Raycast(castPoint.position, direction, out hit, currentGun.maxDistance)) //if we hit an object with our bullet
            {
                trail = Instantiate(bulletTrail, currentGunTip.position, Quaternion.identity); //start a bullet trail effect

                StartCoroutine(SpawnBullet(trail, hit.point, hit)); //spawn our bullet
            }
            else // if we shoot, but we don't hit anything (if we shoot into the air at no objects, 
                 //we still want to show our bullet trail)
            {
                trail = Instantiate(bulletTrail, currentGunTip.position, Quaternion.identity);

                StartCoroutine(SpawnBullet(trail, castPoint.position + direction * (currentGun.maxDistance / 2), hit)); // sets the point of where our raycast would have ended up if it hit anything (point in the air)
                                   
            }
        }
        currentGun.currentAmmo--;
        

        //Debug.Log(hit.point);

        //currentGun.currentAmmo--;
        timeSinceLastShot = 0;
        
        if (currentGun.alternateFire)
        {
            currentGunTip = currentGunTip == gunTip ? alternateGunTip : gunTip;
        }
            
    }
    private IEnumerator SpawnBullet(TrailRenderer trail, Vector3 hitPos, RaycastHit hit)
    {
        float time = 0;
        Transform _gunTip = currentGunTip; // caches the value of the gun tip so coroutines can exist simultaneously
        

        while (time < 1)
        {
            Vector3 startPosition = _gunTip.position;
            trail.transform.position = Vector3.Lerp(startPosition, hitPos, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hitPos;
        
        Destroy(trail.gameObject, trail.time);
        
        /*IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(currentGun.damage);
            StartCoroutine(PlayParticles(gunshotSparksEnemy, hitPos, gunshotSparks.transform.rotation));
        }
        else if (currentGun != guns[1])
        {
            if (playSparks)
            {
                StartCoroutine(PlayParticles(gunshotSparks, hitPos, gunshotSparks.transform.rotation));
            }
            
        }*/

        
        
    }

    private IEnumerator ReloadGun()
    {
        reloading = true;
        yield return new WaitForSeconds(currentGun.reloadTime);
        currentGun.currentAmmo = currentGun.magCapacity;
        reloading = false;
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
    private void OnEnable()
    {
        InputRouting.Instance.input.Player.Reload.performed += ctx =>
        {
            if (currentGun.currentAmmo < currentGun.magCapacity && !reloading)
            {
                StartCoroutine(ReloadGun());
            }
        };
    }

    private void OnDisable()
    {
        if (buttonSet) DisableFireButtonListeners();
        InputRouting.Instance.input.Player.Reload.performed -= ctx =>
        {
            if (currentGun.currentAmmo < currentGun.magCapacity && !reloading)
            {
                StartCoroutine(ReloadGun());
            }
        };
    }
#endregion
}
