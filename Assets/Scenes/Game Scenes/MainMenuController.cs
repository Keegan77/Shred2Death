using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SubMenu[] subMenus;
    private SubMenu currentSubMenu;
    
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
