using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Activates when the player enters the trigger while boosting.
/// </summary>
public class CheckBoostTouch : Sensor
{
    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Player") && other.GetComponentInParent<PlayerBase> () != null)
        {
            //Debug.Log (Enemy.playerReference.playerbase.movement.currentlyBoosting);

            if (Enemy.playerReference.playerbase.movement.currentlyBoosting)
            {
                Activate ();
            }

        }
    }
}
