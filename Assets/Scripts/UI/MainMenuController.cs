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
    [SerializeField] private List<GameObject> leftSideUIObjects;
    [SerializeField] private float buttonOffScreenXValue;

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
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(backgroundSceneEnvironmentToLoad)); // ensures lighting data will be taken from bg scene
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartDisableMenuSequence() //can't call a coroutine from a UI Button click, so this is a workaround
                                           //(could probably use an async workflow to make this better)
    {
        StartCoroutine(SlideOutUI());
    }
    
    private IEnumerator SlideOutUI()
    {
        if (currentSubMenu != null)
        {
            ChangeSubMenu(currentSubMenu); // this just gets rid of the current submenu
        }

        foreach (var UIElement in leftSideUIObjects)
        {
            UIElement.GetComponent<BounceUI>().targetXPosition = buttonOffScreenXValue;
            //if try get component menu button behaviour
            if (UIElement.TryGetComponent(out MenuButtonBehaviour menuButtonBehaviour))
            {
                menuButtonBehaviour.DisableHover();
            }
            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(1);
        ASyncLoader asyncLoader = FindObjectOfType<ASyncLoader>();
        asyncLoader.StartCoroutine(asyncLoader.LoadLevelAsync(3));
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