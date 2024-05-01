using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunfireHandler : MonoBehaviour
{
    private float timeSinceLastShot;
     public GunData startingGun; 
    private GunData currentGun;
    [SerializeField] private SceneDataForGun startingGunSceneData;
    private SceneDataForGun currentGunSceneData;

    private bool buttonSet; // true if we've set a button listener (we have a non-automatic weapon)

    [SerializeField] private Recoil cameraRecoil;
    
    [SerializeField] private TrailRenderer bulletTrail;
    [Header("Transforms")] 
    public Transform castPoint;
    [SerializeField] private Transform forwardFromPlayerPoint;
    [SerializeField] private PlayerHUD playerHUD;
    
    private Transform[] currentGunTips;
    private Recoil[] currentGunRecoilScripts;
    private Recoil currentGunRecoilScript;

    private Transform currentGunTip;

    private bool fireDisabled = false;

    public bool shootInGunDirection = false; //by default we shoot at the middle point of the camera,
                                             //extended in the dir of the camera
    private Vector3 currentGunRecoil;

    private void Awake()
    {
        currentGun = startingGun;
        SetGunRecoil(new Vector3(currentGun.gunRecoilX, currentGun.gunRecoilY, currentGun.gunRecoilZ));
        currentGunSceneData = startingGunSceneData;
        currentGunTips = currentGunSceneData.GetGunTips();
        currentGunRecoilScripts = currentGunSceneData.GetRecoilObjects();
        
        SetUpGun();
    }

    private void SetUpGun() // called on start and on gun switch
    {
        if (buttonSet) // if the previous weapon wasn't automatic, this disables that weapon's button input
        {
            DisableFireButtonListeners();
        }
        playerHUD.SetCrosshair(currentGunSceneData.GetCrosshair());
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

    public void DisablePlayerFire(bool disabled)
    {
        fireDisabled = disabled;
    }
    
    private void Fire()
    {
        if (currentGun.currentAmmo <= 0) return;
        if (fireDisabled) return;
        
        ExecuteGunshot(); // was seperated to allow for flexibility
        
        currentGun.currentAmmo--;
        
        timeSinceLastShot = 0;
        
        playerHUD.SetAmmoUI(currentGun.currentAmmo);
        
        if (currentGun.alternateFire)
        {
            SwitchAlternateFire();
        }
    }

    public void SetGunRecoil(Vector3 recoil)
    {
        currentGunRecoil = recoil;
    }
    
    public void SwitchAlternateFire()
    {
        currentGunTip = currentGunTip == currentGunTips[0] ? currentGunTips[1] : currentGunTips[0];
        currentGunRecoilScript = currentGunRecoilScript == currentGunRecoilScripts[0] ? currentGunRecoilScripts[1] : currentGunRecoilScripts[0];
    }
    
    public void ExecuteGunshot(RaycastHit overrideHit = default, Vector3 overrideStartPoint = default, bool useRecoil = true, bool useSound = true)
    {
        RaycastHit hit = new RaycastHit(); //instantiate our raycast ref
        TrailRenderer trail; // instantiate our gun trail

        if (useRecoil)
        {
            cameraRecoil.FireRecoil(currentGun.camRecoilX, currentGun.camRecoilY, currentGun.camRecoilZ); // apply recoil
            currentGunRecoilScript.FireRecoil(currentGunRecoil.x, currentGunRecoil.y, currentGunRecoil.z);
        }

        int randInt = Random.Range(0, currentGun.fireSounds.Count);
        
        if (useSound) 
            ActionEvents.PlayerSFXOneShot?.Invoke(currentGun.fireSounds[randInt], currentGun.delayPerAudioClip[randInt]); // play a random fire sound

        void DoGunCasts(Vector3 hitPos, RaycastHit hit)
        {
            var startPoint = currentGunTip.position;
            if (overrideStartPoint != default)
            {
                startPoint = overrideStartPoint;
            }
            trail = Instantiate(bulletTrail, startPoint, Quaternion.identity);
            StartCoroutine(SpawnBullet(trail, hitPos, startPoint, hit));
        }
        
        if (overrideHit.point != default) // if we have an override hit point, use that
        {
            DoGunCasts(overrideHit.point, overrideHit);
            return;
        }
        
        for (int i = 0; i < currentGun.bulletsInOneShot; i++) 
        {
            Vector3 direction = GetDirection(); 
            if (Physics.Raycast(castPoint.position, direction, out hit, currentGun.maxDistance)) //if we hit an object with our bullet
            {
                DoGunCasts(hit.point, hit);
            }
            else // if we shoot, but we don't hit anything (if we shoot into the air at no objects, 
                //we still want to show our bullet trail)
            {
                DoGunCasts(castPoint.position + direction * currentGun.maxDistance, hit); // sets the point of
                                                                                          // where our raycast would
                                                                                          // have ended up if it hit
                                                                                          // anything (point in the air)
            }
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
    public void ShootFromGunForward(bool enabled)
    {
        shootInGunDirection = enabled;
    }
    
    private Vector3 GetDirection()
    {
        Vector3 direction = castPoint.forward;
        if (shootInGunDirection)
        {
            direction = currentGunTip.forward;
        }

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
    
    public void SetGunTip(Transform tip)
    {
        currentGunTip = tip;
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
        playerHUD.SetAmmoUI(currentGun.currentAmmo);
        
        if (currentGun.currentAmmo > currentGun.magCapacity) currentGun.currentAmmo = currentGun.magCapacity;
    }

    private void OnEnable()
    {
        InputRouting.Instance.input.Player.Fire.performed += ctx =>
        {
            if (currentGun.currentAmmo <= 0)
            {
                int rand = Random.Range(0, currentGun.outOfAmmoSounds.Count);
            
                ActionEvents.PlayerSFXOneShot?.Invoke(currentGun.outOfAmmoSounds[rand], currentGun.delayPerAudioClip[rand]);
            }
        };
        ActionEvents.OnTrickCompletion += IncreaseAmmo;
    }

    private void OnDisable()
    {
        if (buttonSet) DisableFireButtonListeners();
        ActionEvents.OnTrickCompletion -= IncreaseAmmo;
        InputRouting.Instance.input.Player.Fire.performed -= ctx =>
        {
            if (currentGun.currentAmmo <= 0)
            {
                int rand = Random.Range(0, currentGun.outOfAmmoSounds.Count);
            
                ActionEvents.PlayerSFXOneShot?.Invoke(currentGun.outOfAmmoSounds[rand], .2f);
            }
        };
    }
#endregion
}
