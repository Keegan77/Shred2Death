using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Enemy State Machine controls the behavior of enemies. If they are in state X, they will move like this and use these senses.
/// Statemachine handles the switching of states and serves as in interface to the Enemy Script.
/// </summary>
public class Enemy_StateMachine : MonoBehaviour
{
    #region VARIABLES
    //[SerializeField] Enemy_State initialState;

    GameObject statesObject;

    [Header("States")]
    public Enemy_State stateCurrent;

    [Tooltip("What state does this enemy enter when they spawn in? Usually set to a move_to_point script")]
    public Enemy_State stateEntry;

    [Header ("Navigation")]
    public Vector3 travelPoint;
    public GameObject travelTarget;

    #endregion



    #region SCRIPT FUNCTIONS
    public void machineUpdate ()
    {
        stateCurrent.machineUpdate ();
    }

    public void machinePhysics ()
    {
        stateCurrent.machinePhysics ();
    }

    public void transitionState (Enemy_State s)
    {
        Debug.Log ("Transitioning from " + stateCurrent + " to " + s);
        stateCurrent.Exit ();

        stateCurrent = s;

        stateCurrent.Enter ();
    }

    public void OnSensorActivated ()
    {
        stateCurrent.onPlayerSensorActivated ();
    }

    public void OnSensorDeactivated ()
    {
        stateCurrent.onPlayerSensorDeactivated ();
    }

    #endregion
   
    
    
    #region SETUP

    private void Awake ()
    {
        //currentState = initialState;
        statesObject = transform.Find ("States").gameObject;
    }

    #endregion


}
