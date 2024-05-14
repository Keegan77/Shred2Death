using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    #region References

    [Header("References")] [SerializeField]
    private PlayerBase player;
    [Tooltip("See the tooltips on the StatsWidget object to see how to use the widget.")]
    public StatsWidget stats;

    [Tooltip("Container for the per weapon crosshairs. Contains reference variables to each crosshair")]
    public Crosshairs crosshair;

    public PlayerHUDSubMenu menuGameOver;
    public PlayerHUDSubMenu menuPause;
    
    public GameObject subMenuContainer;
    public GameObject widgetContainer;

    public GameObject grindDisplayButton;
    private Canvas playerCanvas;
    #endregion

    public bool gamePaused = false;
    float currentTimeScale = 1;
    private GameObject[] children;

    private void Awake()
    {
        subMenuContainer = transform.Find("SubMenus").gameObject;
        widgetContainer = transform.Find("Widgets").gameObject;
        playerCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        SwitchAmmoBar(player.gunfireHandler.GetCurrentGunData().currentAmmo, player.gunfireHandler.startingGun);
    }
    
    public void SwitchAmmoBar(float currentAmmo, GunData newGun)
    {
        stats.ammoBar.currentValue = currentAmmo;
        stats.ammoBar.maxValue = newGun.magCapacity;
        cur = currentAmmo;
        stats.ammoText.text = $"{currentAmmo}";
    }

    private void OnEnable()
    {
        ActionEvents.TurnOffPlayerUI += TurnOffUI;
        ActionEvents.TurnOnPlayerUI += TurnOnUI;
    }

    private void OnDisable()
    {
        ActionEvents.TurnOffPlayerUI -= TurnOffUI;
        ActionEvents.TurnOnPlayerUI -= TurnOnUI;
    }

    private void Update()
    {
        stats.ammoBar.currentValue = Mathf.Lerp(stats.ammoBar.currentValue, cur, Time.deltaTime * 5);
    }

    public List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).gameObject.CompareTag("SkipUIDeactivate")) continue;
            result.Add(parent.transform.GetChild(i).gameObject);
        }
        return result;
    }

    private void TurnOffUI()
    {
        foreach (GameObject menuComponent in GetAllChildren(widgetContainer))
        {
            if (menuComponent == gameObject) return;
            menuComponent.SetActive(false);
        }
    }
    
    private void TurnOnUI()
    {
        foreach (GameObject menuComponent in GetAllChildren(widgetContainer))
        {
            if (menuComponent == gameObject) return;
            menuComponent.SetActive(true);
        }
    }
    

    #region active HUD
    public void SetCrosshair(UnityEngine.UI.RawImage c)
    {
        foreach (Transform t in crosshair.transform)
        {
            t.gameObject.SetActive(false);
        }

        c.gameObject.SetActive(true);
    }

    private float cur;
    public void SetAmmoUI(float currentAmmo)
    {
        stats.ammoBar.maxValue =  player.gunfireHandler.GetCurrentGunData().magCapacity;
        cur = currentAmmo;
        stats.ammoText.text = $"{currentAmmo}";
    }
    #endregion

    #region Menuing
    public void openMenu(PlayerHUDSubMenu menu)
    {
        InputRouting.Instance.DisableInput();
        foreach (Transform t in subMenuContainer.transform)
        {
            t.gameObject.SetActive(false);
        }

        menu.OnActivated();
    }
    
    private void CloseMenu()
    {
        InputRouting.Instance.EnableInput();
        foreach (Transform t in subMenuContainer.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void ToggleGamePaused()
    {
        //pause the game, recording the timescale
        if (!gamePaused)
        {
            TurnOffUI();
            openMenu(menuPause);
            Cursor.lockState = CursorLockMode.None;

            currentTimeScale = Time.timeScale;
            BulletTimeManager.Instance.ChangeBulletTime(0);
        }

        //unpause the game, setting the timescale to the proper speed
        else
        {
            TurnOnUI();
            CloseMenu();
            Cursor.lockState = CursorLockMode.Locked;
            BulletTimeManager.Instance.ChangeBulletTime(1);
        }

        gamePaused = !gamePaused;
    }

    #endregion

    #region Scene Management
    public void Scene_ReturnToTitle()
    {
        GameManager.instance.SetGameState(GameManager.GameState.MainMenu);
        SceneManager.LoadScene("NewMainMenu");
        Time.timeScale = 1;
    }

    public void Scene_RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //player.movement = null;
        Destroy(player);
        Time.timeScale = 1;
    }

    public void Scene_LoadLevel(string l)
    {
        SceneManager.LoadScene(l);
        Time.timeScale = 1;
    }
    #endregion
}
