using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneStateManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> disableIfMainMenuActive;
    [SerializeField] private List<GameObject> enableIfMainMenuActive;

    private void Start()
    {
        if (GameManager.instance.GetGameState() == GameManager.GameState.MainMenu)
        {
            MakeSceneChanges();
        }
    }

    public void MakeSceneChanges()
    {
        foreach (var obj in disableIfMainMenuActive)
        {
            obj.SetActive(false);
        }

        foreach (var obj in enableIfMainMenuActive)
        {
            obj.SetActive(true);
        }
    }
}
