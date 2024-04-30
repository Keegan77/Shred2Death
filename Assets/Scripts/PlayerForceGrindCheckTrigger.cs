using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForceGrindCheckTrigger : MonoBehaviour
{
    private bool triggered;
    [SerializeField] float grindSpeed;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            PlayerBase player = other.gameObject.GetComponentInParent<PlayerBase>();
            player.OverrideGrindSpeed(grindSpeed);
            player.CheckAndSetSpline();
        }
    }
}
