using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private void StartFreezeCoroutine(InputAction inputAction, GameObject textForPopUp)
    {
        StartCoroutine(WaitForInput(inputAction, textForPopUp));
    } 
    
    private IEnumerator WaitForInput(InputAction inputAction, GameObject textForPopUp)
    {
        BulletTimeManager.Instance.ChangeBulletTime(0);
        textForPopUp.SetActive(true);
        messagePopUp.MoveToEndPosition();
        inputAction.Enable();
        yield return new WaitUntil(() => inputAction.triggered);
        messagePopUp.MoveToStartPosition();
        Time.timeScale = 1;
        inputAction.Disable();
        yield return new WaitForSeconds(1);
        textForPopUp.SetActive(false);
    }
}
