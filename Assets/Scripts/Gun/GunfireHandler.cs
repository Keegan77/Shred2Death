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

    [SerializeField] private Transform castPoint;
    
    [SerializeField] private TrailRenderer bulletTrail;

    [SerializeField] private Transform gunTip;
    
    [SerializeField] private Transform alternateGunTip;

    private Transform currentGunTip;

    private void Start()
    {
        currentGunTip = gunTip;
        bulletTrail.time = currentGun.bulletLerpTime;
        if (!currentGun.automatic)
        {
            SetButtonListeners(); // if gun isn't auto then we just set gunfire to our button down input
        }
        
    }

    private bool CanShoot() =>
        timeSinceLastShot > currentGun.timeBetweenShots; // if we're done firing the last shot

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (currentGun.automatic && CanShoot() && InputRouting.Instance.GetFireHeld())
        {
            Fire();
        } // if the gun is automatic, and we can shoot, and we're holding the fire button, then fire
    }

    private void SetButtonListeners()
    {
        InputRouting.Instance.input.Player.Fire.performed += ctx =>
        {
            if (CanShoot())
            {
                Fire();
            }
        };
    }
    
    private void OnDisable()
    {
        DisableButtonListeners();
    }
    
    private void DisableButtonListeners()
    {
        InputRouting.Instance.input.Player.Fire.performed -= ctx => Fire();
    }

    private void Fire()
    {
        RaycastHit hit; //instantiate our raycast ref
        TrailRenderer trail; // instantiate our gun trail
        //recoilScript.FireRecoil(); // camera recoil
        //gunObjRecoil.FireGunRecoil(); // gun recoil
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
        Transform _gunTip = currentGunTip; // cache the value of the gun tip

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
    
    private Vector3 GetDirection()
    {
        Vector3 direction = castPoint.forward;

        direction += new Vector3(Random.Range(-currentGun.spreadX, currentGun.spreadX),
                                 Random.Range(-currentGun.spreadY, currentGun.spreadY),
                                 Random.Range(-currentGun.spreadZ, currentGun.spreadZ));
        direction.Normalize();
        return direction;
    }
    
}
