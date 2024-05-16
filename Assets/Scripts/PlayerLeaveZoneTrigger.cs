using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerLeaveZoneTrigger : MonoBehaviour
{
    private bool leftZone;
    private PlayerBase player;
    private float cachedFov;
    private Vector3 cachedPos;
    [SerializeField] private float timeAfterTriggerEnterToFadeOut;
    [SerializeField] private int buildIndexToGoTo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !leftZone)
        {
            player = other.gameObject.GetComponentInParent<PlayerBase>();
            leftZone = true;
            player.movement.ToggleAutoMove(true);
            InputRouting.Instance.DisableInput();
            cachedFov = Helpers.MainCamera.fieldOfView;
            cachedPos = Helpers.MainCamera.transform.position;
            Helpers.MainCamera.transform.parent = null;
            Helpers.MainCamera.fieldOfView = cachedFov;
            Helpers.MainCamera.transform.position = cachedPos;
            ActionEvents.TurnOffPlayerUI?.Invoke();
            StartCoroutine(GoToNextZone());
        }
    }

    private void Update()
    {
        if (leftZone)
        {
            Helpers.MainCamera.transform.LookAt(player.transform);
        }
    }
    
    private IEnumerator GoToNextZone()
    {
        yield return new WaitForSeconds(timeAfterTriggerEnterToFadeOut);
        ActionEvents.LoadNewSceneEvent?.Invoke(buildIndexToGoTo, 1);
        
    }
}
