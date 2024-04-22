using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] InputAction tutorialAction;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        ActionEvents.FreezeAndWaitForInput?.Invoke(tutorialAction);
    }
}
