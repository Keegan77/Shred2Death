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
    #region PARAMETERS
    //[SerializeField] Enemy_State initialState;

    #endregion

    #region SCRIPT VARIABLES
    GameObject statesObject;

    [SerializeField] Enemy_State currentState;
    #endregion



    #region SCRIPT FUNCTIONS
    public void machineUpdate ()
    {
        currentState.machineUpdate ();
    }

    public void machinePhysics ()
    {
        currentState.machinePhysics ();
    }

    public void transitionState (Enemy_State s)
    {
        Debug.Log ("Transitioning from " + currentState + " to " + s);
        currentState.Exit ();

        currentState = s;

        currentState.Enter ();
    }

    public void sensorActivated ()
    {
        currentState.onPlayerSensorActivated ();
    }

    public void sensorDeactivated ()
    {
        currentState.onPlayerSensorDeactivated ();
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
