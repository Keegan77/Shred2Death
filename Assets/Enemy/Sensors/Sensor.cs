using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sensors are passive triggers that send a signal to an enemy's state machine and has them do things.
/// Sensors are a 2nd child of the Enemy class, and will signal upwards to Enemy's StateMachine.
/// </summary>
public class Sensor : MonoBehaviour
{
    protected virtual void Activate ()
    {
        Debug.Log ("Activated");
        transform.parent.parent.GetComponent<Enemy_StateMachine> ().sensorActivated ();
    }

    protected virtual void Deactivate ()
    {
        Debug.Log ("Deactivated");
        transform.parent.parent.GetComponent<Enemy_StateMachine>().sensorDeactivated ();
    }
}
