using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SubMenu[] subMenus;
    [Tooltip("Corresponds to the index of the scene you would like to use as the background environment" +
             " for the main menu.")]
    [SerializeField] private int backgroundSceneEnvironmentToLoad;
    private SubMenu currentSubMenu;

    private void Awake()
    {
        GameManager.instance.SetGameState(GameManager.GameState.MainMenu);
        StartCoroutine(LoadBackgroundScene());

    }
    private IEnumerator LoadBackgroundScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(backgroundSceneEnvironmentToLoad, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(backgroundSceneEnvironmentToLoad));
        Cursor.lockState = CursorLockMode.None;
    }

    public void ChangeSubMenu(SubMenu newSubMenu)
    {
        foreach (var menu in subMenus)
        {
            if (menu == newSubMenu && currentSubMenu != newSubMenu)
            {
                currentSubMenu = newSubMenu;
                menu.GetComponent<BounceUI>().targetXPosition = menu.GetComponent<MenuButtonBehaviour>().selectedPos;
            }
            else
            {
                menu.GetComponent<BounceUI>().targetXPosition = menu.GetComponent<MenuButtonBehaviour>().restingPos;
                if (currentSubMenu == menu)
                {
                    currentSubMenu = null;
                }
            }
        }
        
    }
    
    public void SaveSensitivity(Slider sliderObj)
    {
        float newSens = Mathf.Lerp(25, 300, sliderObj.value);
        PlayerPrefs.SetFloat("Sensitivity", newSens);
        Debug.Log("Sensitivity saved as " + newSens);
    }

}
