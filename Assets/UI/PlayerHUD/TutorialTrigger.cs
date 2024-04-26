using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] InputAction tutorialAction;
    [SerializeField] private GameObject triggerTextObject;
    private bool popUpShown;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (popUpShown) return;
        popUpShown = true;
        
        ActionEvents.FreezeAndWaitForInput?.Invoke(tutorialAction, triggerTextObject);
    }
}
