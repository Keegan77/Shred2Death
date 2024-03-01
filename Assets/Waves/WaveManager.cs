using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//After going through and doing validaiton scripting, ArenaComplete might not need to exist
public class WaveManager : MonoBehaviour
{
    #region PARAMETERS

    [Header ("Events")]

    //A list of things that can happen when the encounter starts.
    //It's most commonly going to be closing the gates,
    //but we can leave the door open for other things to happen.
    [Tooltip("When the arena is activated by entering it, which events happen? (Attach to WaveManager as components from Assets/Waves/Wave Events)")]
    [SerializeField]
    Wave_Event[] openingEvents;

    //As with the opening events, these are things that happen when the arena is completed.
    [Tooltip ("When all waves in this arena have been cleared, which events happen? (Attach to WaveManager as components from Assets/Waves/Wave Events)")]
    [SerializeField]
    Wave_Event[] closingEvents;

    [Header("Waves")]
    [SerializeField] Wave[] waves;

    #endregion



    #region SCRIPT VARIABLES

    bool areaEnabled = true;
    bool areaComplete = false;

    int currentWave = 0;
    int remainingEnemies = 0;

    #endregion


    #region SETUP
    private void Start ()
    {
        //If the wavemanager is set up incorrectly, disable it.
        if (!ValidateArena())
        {
            Debug.LogWarning($"Arena {gameObject} has been disabled due to incomplete settings");

            areaEnabled = false;
        }
        
    }

    //Algorithm for checking through the arena for invalid settings
    bool ValidateArena()
    {
        bool isValid = true;
        //Run a check to see if any of the waves are wrong (0 enemies, or no prefab)
        if (waves.Length == 0)
        {
            Debug.LogError($"No Waves set on {gameObject.name}");
            isValid = false;
        }
        else
        {
            for (int w = 0; w < waves.Length; w++)
            {
                if (waves[w].getEnemies().Length == 0)
                {
                    Debug.LogError($"Arena {gameObject.name} Wave {w} on {gameObject.name} is empty");
                    isValid = false;
                }
                else
                {
                    for (int e = 0; e < waves[w].getEnemies().Length; e++)
                    {
                        if (waves[w].getEnemies()[e].enemy == null)
                        {
                            Debug.LogError($"Arena {gameObject.name} Wave {w} Group {e} has no enemy set");
                            isValid = false;
                        }

                        if (waves[w].getEnemies()[e].spawnPoint == null)
                        {
                            Debug.LogError($"Arena {gameObject.name} Wave {w} Group {e} has no spawn point set");
                            isValid = false;
                        }
                    }
                }
            }
        }

        return isValid;
    }

    #endregion

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player") && areaComplete == false && areaEnabled)
        {
            Debug.Log ("Player has entered the arena");
            areaEnabled = false;

            StartCoroutine (beginEncounter ());
        }
    }

    //Play through the opening events and then begin the first wave.
    IEnumerator beginEncounter ()
    {
        //Play the opening events of the arena
        foreach (Wave_Event e in openingEvents)
        {
            e.event_Open ();
            yield return e.eventDelay ();
        }

        //Once the opening events are complete, begin the first wave
        StartCoroutine(playWave ());
    }

    //Play through the closing events.
    IEnumerator finishEncounter ()
    {
        foreach (Wave_Event e in closingEvents)
        {
            e.event_Close ();
            yield return e.eventDelay ();
        }

        areaComplete = true;
        areaEnabled = false;
    }

    //Instantiate Enemies at fixed intervals
        //When each of those enemies spawn, have them move to enter the arena.
    IEnumerator playWave ()
    {
        Debug.Log("Beginning wave " + currentWave + " With " + waves[currentWave].getEnemyCount() + " enmies");
        remainingEnemies = waves[currentWave].getEnemyCount();

        foreach (Wave.Row row in waves[currentWave].getEnemies())
        {
            StartCoroutine(spawnEnemies (row));
        }

        yield return new WaitForSeconds (1);
    }

    //Called by playWave. Each row in the wave begins depositing its enemies simultaneously.
    //Enemies spawn at a fixed interval after a certain amount of time declared by the row.
    IEnumerator spawnEnemies (Wave.Row row)
    {
        yield return new WaitForSeconds (row.spawnDelay);

        //Spawn enemies and direct them to move towards the entry point.
        for (int i = 0; i < row.count; i++)
        {
            GameObject e = Instantiate (row.enemy, row.spawnPoint.transform.GetChild(0).position, row.spawnPoint.transform.GetChild(0).rotation, transform.Find ("Enemies"));
            
            Enemy_StateMachine esm = e.GetComponent<Enemy_StateMachine>();

            esm.travelTarget = row.spawnPoint.transform.GetChild (2).gameObject;
            esm.travelPoint = row.spawnPoint.transform.GetChild (2).position;

            //Hopefully the NavMeshAgent is loaded after instantiation
            //e.GetComponent<Enemy>().agent = e.gameObject.GetComponent<NavMeshAgent> (); 

            esm.transitionState (esm.stateEntry);
            e.GetComponent<Enemy> ().SetManager (this);

            yield return new WaitForSeconds (row.interval);
        }
        
    }

    //Each time an enemy dies, take count of the current wave
        //if its empty move on to the next wave
            //if there is no next wave end the event
    public void removeEnemy ()
    {
        remainingEnemies--;
        Debug.Log ("Enemy removed. " + remainingEnemies + " remaining in wave " + currentWave);

        //All enemies have been cleared
        if (remainingEnemies == 0)
        {
            currentWave++;

            //Was this the last wave?
            if (currentWave == waves.Length)
            {
                StartCoroutine(finishEncounter ());
            }
            //if not, start the next wave.
            else
            {
                StartCoroutine(playWave ());
            }
        }
    }
}
