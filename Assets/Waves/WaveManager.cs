using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region PARAMETERS

    [SerializeField] Wave[] waves;

    #endregion



    #region SCRIPT VARIABLES

    bool areaEnabled = false;
    bool areaComplete = false;

    int currentWave = 0;
    int remainingEnemies = 0;

    #endregion



    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player") && areaComplete == false && areaEnabled == false)
        {
            Debug.Log ("Player has entered the arena");

            //Begin the wave
        }
    }

    private void Start ()
    {
        //Run a check to see if any of the waves are wrong (0 enemies, or no prefab)

    }

    //Instantiate Enemies at fixed intervals
        //When each of those enemies spawn, have them move to enter the arena.

    //Each time an enemy dies, take count of the current wave
        //if its empty move on to the next wave
            //if there is no next wave end the event
}
