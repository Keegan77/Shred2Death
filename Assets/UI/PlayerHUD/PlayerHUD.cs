using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    #region References
    [Header("References")]
    [Tooltip("See the tooltips on the StatsWidget object to see how to use the widget.")]
    public StatsWidget stats;

    public PlayerHUDSubMenu menuGameOver;
    public PlayerHUDSubMenu menuPause;

    private GameObject subMenuContainer;
    #endregion

    bool gamePaused = false;
    float currentTimeScale = 1;

    private void Awake()
    {
        subMenuContainer = transform.Find("SubMenus").gameObject;
        subMenuContainer.SetActive(false);
    }

    #region Menuing
    public void openMenu(PlayerHUDSubMenu menu)
    {
        foreach (Transform t in subMenuContainer.transform)
        {
            t.gameObject.SetActive(false);
        }

        menu.OnActivated();
    }

    public void ToggleGamePaused()
    {
        //pause the game, recording the timescale
        if (!gamePaused)
        {
            openMenu(menuPause);
            subMenuContainer.SetActive(true);

            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        //unpause the game, setting the timescale to the proper speed
        else
        {
            Time.timeScale = currentTimeScale;
            subMenuContainer.SetActive(false);
        }

        gamePaused = !gamePaused;
    }

    #endregion

    #region Scene Management
    public void Scene_ReturnToTile()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void Scene_RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Scene_LoadLevel(string l)
    {
        SceneManager.LoadScene(l);
        Time.timeScale = 1;
    }
    #endregion
}
