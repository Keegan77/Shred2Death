using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeOpacity : MonoBehaviour
{
    private Image blackScreen;

    private void Awake() //do not destroy on load
    {
        DontDestroyOnLoad(gameObject);
        
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        ActionEvents.FadeToBlack += FadeToBlack;
    }
    
    private void OnDisable()
    {
        ActionEvents.FadeToBlack -= FadeToBlack;
    }

    private void Start()
    {
        blackScreen = GetComponentInChildren<Image>();
        blackScreen.gameObject.SetActive(false);
    }

    private async void FadeToBlack(bool fadeBlackIn, float fadeDuration)
    {
        blackScreen.gameObject.SetActive(true);
        float t = 0;
        float startOpacity = fadeBlackIn ? 0 : 1;
        float endOpacity = fadeBlackIn ? 1 : 0;
        Color curColor = blackScreen.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, endOpacity, t / fadeDuration);
            curColor.a = newOpacity;
            blackScreen.color = curColor;
            await Task.Yield();
        }
    }
}
