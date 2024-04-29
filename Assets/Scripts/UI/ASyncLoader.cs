using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASyncLoader : MonoBehaviour
{
    private void OnEnable()
    {
        ActionEvents.LoadNewSceneEvent += StartLevelLoad;
    }

    private void OnDisable()
    {
        ActionEvents.LoadNewSceneEvent -= StartLevelLoad;
    }

    private void StartLevelLoad(int sceneIndex, float duration)
    {
        StartCoroutine(LoadLevelAsync(sceneIndex, duration));
    }
    
    public IEnumerator LoadLevelAsync(int sceneIndex, float duration)
    {
        //unload background scene
        
        ActionEvents.FadeToBlack?.Invoke(true, duration);
        yield return new WaitForSeconds(duration);
        //yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);//Unloads the curr scene after the fade to black
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        GameManager.instance.SetGameState(GameManager.GameState.InGame);
        ActionEvents.FadeToBlack?.Invoke(false, 1);
    }
}
