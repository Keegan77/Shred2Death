using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Enemy_StateMachine))]
public class Enemy : MonoBehaviour
{

    #region PAREMETERS

    [Header("Stats")]
    public int health;
    public float movementSpeed;

    #endregion

    #region SCRIPT VARIABLES
    Enemy_StateMachine stateMachine;

    WaveManager waveManager;

    #endregion

    #region SETUP
    void Start ()
    {
        stateMachine = GetComponent<Enemy_StateMachine>();
    }

    public void setManager (WaveManager w)
    {
        waveManager = w;
    }
    #endregion

    #region SCRIPT FUNCTIONS

    public void takeDamage ()
    {
        waveManager.removeEnemy ();

        Destroy (gameObject);
    }

    #endregion

    #region UNITY FUNCTIONS
    void Update ()
    {
        if (stateMachine.enabled)
        {
            stateMachine.machineUpdate();
        }
    }

    private void FixedUpdate ()
    {
        if (stateMachine.enabled)
        {
            stateMachine.machinePhysics();
        }

    }

    #endregion
}
