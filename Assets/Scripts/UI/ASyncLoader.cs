using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASyncLoader : MonoBehaviour
{
    public IEnumerator LoadLevelAsync(int sceneIndex)
    {
        //unload background scene
        ActionEvents.FadeToBlack?.Invoke(true, 2);
        yield return new WaitForSeconds(2);
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);//Unloads the curr scene after the fade to black
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
        ActionEvents.FadeToBlack?.Invoke(false, 1);
    }
}
