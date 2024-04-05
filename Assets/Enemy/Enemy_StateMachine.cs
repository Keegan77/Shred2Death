using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Enemy State Machine controls the behavior of enemies. If they are in state X, they will move like this and use these senses.
/// Statemachine handles the switching of states and serves as in interface to the Enemy Script.
/// 
/// Additionally, it handles as a storage for important information used by states, such as directional goals and timers.
/// </summary>
public class Enemy_StateMachine : MonoBehaviour
{
    #region VARIABLES
    //[SerializeField] Enemy_State initialState;

    public GameObject statesObject;

    [Header("States")]
    public Enemy_State stateCurrent;
    public Enemy_State statePrevious;

    [Tooltip("What state does this enemy enter when they spawn in? Usually set to a move_to_point script")]
    public Enemy_State stateEntry;


    [Header("Control Parameters")]

    [Tooltip("Interval in seconds between AI updates")]
    [Min(0.1f)]
    public float aiFrequency = 1f;
    float aiTimeKeeper = 0f;

    

    //updateEnabled controls whether or not the stateMachien will run AI checks.
    public bool aiUpdateEnabled
    {
        get { return _aiUpdateEnabled; }
        set { _aiUpdateEnabled = value; aiTimeKeeper = Time.time; }
    }
    private bool _aiUpdateEnabled = true;

    /// <summary>
    /// Some states will need to keep track of time.
    /// 
    /// </summary>
    public float timerCurrentState;

    [Header ("Navigation")]
    public Vector3 travelPoint;
    public GameObject travelTarget;

    #endregion

    #region SCRIPT FUNCTIONS
    public void machineUpdate ()
    {
        stateCurrent.machineUpdate ();
        timerCurrentState += Time.deltaTime;
    }

    /// <summary>
    /// Handles timers that 
    /// </summary>
    public void machinePhysics ()
    {
        stateCurrent.machinePhysics ();

        if (aiUpdateEnabled && Time.time - aiTimeKeeper > aiFrequency)
        {
            AIUpdate ();

            aiTimeKeeper = Time.time;
        }
    }

    private void AIUpdate ()
    {
        stateCurrent.AIUpdate ();
    }

    public void transitionState (Enemy_State s)
    {
        timerCurrentState = 0;

        Debug.Log ("Transitioning state to: " + s);
        stateCurrent.Exit ();

        statePrevious = stateCurrent;
        stateCurrent = s;

        stateCurrent.Enter ();
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

    private void Start ()
    {
        aiTimeKeeper = Time.time;
    }

    #endregion


}
