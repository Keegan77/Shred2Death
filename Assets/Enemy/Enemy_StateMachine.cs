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



    [Header("Control Parameters")]

    [Tooltip("Will the enemy update the logic in their current state?")]
    public bool aiUpdate = true;

    [Tooltip("How long does the enemy wait  ")]
    [Min(0.1f)]
    public float aiFrequency = 1f;


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

    private void AiUpdate ()
    {

    }

    public void transitionState (Enemy_State s)
    {
        stateCurrent.Exit ();

        stateCurrent = s;

        stateCurrent.Enter ();
    }

    public void setAIPaused (bool enabled)
    {
        if ( enabled )
        {
            InvokeRepeating()
        }
        else
        {
            CancelInvoke ();
        }
    }


    #endregion

    #region Listeners
    public void OnSensorActivated ()
    {
        stateCurrent.onPlayerSensorActivated ();
    }

    public void OnSensorDeactivated ()
    {
        stateCurrent.onPlayerSensorDeactivated ();
    }

    /// <summary>
    /// Animations have events that are caught by a script.
    /// This signal is brought to the stateMachine and then sent down to the current state.
    /// 
    /// This is usually to signal a bulletPattern to play in line with an animation.
    /// </summary>
    public void OnBullet ()
    {
        stateCurrent.OnBullet();
    }

    /// <summary>
    /// Animations send this signal down to the current state when they are finished.
    /// </summary>
    public void OnAnimationFinished ()
    {
        stateCurrent.OnAnimationFinished ();
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
