using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceDeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBase player = other.GetComponentInParent<PlayerBase>();
            player.stateMachine.SwitchState(player.deathState);   
            //ActionEvents.LoadNewSceneEvent?.Invoke(SceneManager.GetActiveScene().buildIndex, 3);
        }
    }
}
