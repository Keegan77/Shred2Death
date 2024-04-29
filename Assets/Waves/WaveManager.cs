using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


//After going through and doing validaiton scripting, ArenaComplete might not need to exist
public class WaveManager : MonoBehaviour
{
    #region PARAMETERS

    [Header ("Events")]
    //teehee
    [Tooltip ("When the arena is triggered and starts its first wave, what happens? (For some god forsaken reason the engine gives me errors when I name this variable responsibly)")]
    [SerializeField]
    WaveArenaEvent[] eventArenaIsOpened;
    //As with the opening events, these are things that happen when the arena is completed.
    [Tooltip ("When all waves in this arena have been cleared, which events happen? (Attach to WaveManager as components from Assets/Waves/Wave Events)")]
    [SerializeField] WaveArenaEvent[] eventArenaClose;


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
            //for each wave:
            for (int w = 0; w < waves.Length; w++)
            {
                //if there are no rows, the wave is not valid
                if (waves[w].getEnemies().Length == 0)
                {
                    Debug.LogError($"Arena {gameObject.name} Wave {w} on {gameObject.name} is empty");
                    isValid = false;
                }
                //if there are rows: 
                else
                {
                    //For each row in the wave:
                    for (int r = 0; r < waves[w].getEnemies ().Length; r++)
                    {
                        //if no enemies are set to spawn
                        if (waves[w].getEnemies ()[r].count < 1)
                        {
                            Debug.LogWarning ($"<color=#ffff00>Arena</color> <color=#00ff00>{gameObject.name}</color> Wave {w} Group {r} has a count of 0. Setting to 1", gameObject);
                            waves[w].getEnemies ()[r].count = 1;
                        }

                        //if no interval is set
                        if (waves[w].getEnemies()[r].interval < 1)
                        {
                            Debug.LogWarning ($"<color=#ffff00>Arena</color> <color=#00ff00>{gameObject.name}</color> Wave {w} Group {r} has an interval of 0. Setting to 1", gameObject);
                            waves[w].getEnemies ()[r].interval = 1;
                        }

                        //if the enemy is not set
                        if (waves[w].getEnemies()[r].enemy == null)
                        {
                            Debug.LogError($"<color=#ffff00>Arena</color> <color=#00ff00>{gameObject.name}</color> Wave {w} Group {r} has no enemy set");
                            isValid = false;
                        }

                        //if there is no spawn point set
                        if (waves[w].getEnemies()[r].spawnPoint == null)
                        {
                            Debug.LogError($"<color=#ffff00>Arena</color> <color=#00ff00>{gameObject.name}</color> Wave {w} Group {r} has no spawn point set");
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
    [ContextMenu("Begin Encounter")]
    public void devBeginEncounter ()
    {
        if (!Application.isPlaying) return;
        StartCoroutine (beginEncounter ());
    }

    IEnumerator beginEncounter ()
    {
        //Play the opening events of the arena
        foreach (WaveArenaEvent e in eventArenaIsOpened)
        {
            e.arenaEvents.Invoke ();
            yield return new WaitForSeconds (e.eventTime);
        }

        //eventArenaOpen.Invoke ();
        //yield return null;

        //Once the opening events are complete, begin the first wave
        StartCoroutine (playWave ());
    }

    //Play through the closing events.
    IEnumerator finishEncounter ()
    {
        foreach (WaveArenaEvent e in eventArenaClose)
        {
            e.arenaEvents.Invoke ();
            yield return new WaitForSeconds (e.eventTime);
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

            //Set the travel target of the enemy to the spawnpoint object.
            esm.travelTarget = row.spawnPoint.gameObject;
            esm.travelPoint = row.spawnPoint.transform.position;

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
