using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIButtonMessageController : MonoBehaviour
{
    [SerializeField] private BounceUI messagePopUp;
    private bool buttonPressed;
    private void OnEnable()
    {
        ActionEvents.FreezeAndWaitForInput += StartFreezeCoroutine;
    }
    
    private void OnDisable()
    {
        ActionEvents.FreezeAndWaitForInput -= StartFreezeCoroutine;
    }

    private void StartFreezeCoroutine(InputAction inputAction)
    {
        StartCoroutine(WaitForInput(inputAction));
    } 
    
    private IEnumerator WaitForInput(InputAction inputAction)
    {
        Time.timeScale = 0;
        messagePopUp.targetXPosition = 190;
        inputAction.Enable();
        yield return new WaitUntil(() => inputAction.triggered);
        messagePopUp.targetXPosition = -190;
        Time.timeScale = 1;
        inputAction.Disable();
    }
}
