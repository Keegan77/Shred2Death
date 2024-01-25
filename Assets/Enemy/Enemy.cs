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


    #endregion

    #region SCRIPT VARIABLES
    Enemy_StateMachine stateMachine;

    #endregion

    #region SETUP
    void Start ()
    {
        stateMachine = GetComponent<Enemy_StateMachine>();
    }
    #endregion


    void Update()
    {
        if (stateMachine.enabled)
        {
            stateMachine.machineUpdate();
        }
    }
}
