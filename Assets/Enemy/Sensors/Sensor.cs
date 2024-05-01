using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
/// <summary>
/// Sensors are passive triggers that send a signal to an enemy's state machine and has them do things.
/// Sensors are a 2nd child of the Enemy class, and will signal upwards to Enemy's StateMachine.
/// </summary>
public class Sensor : MonoBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    protected virtual void Activate ()
    {
        //Debug.Log ($"{transform.parent.name}: {name} Activated", this);
        //transform.parent.parent.GetComponent<Enemy_StateMachine> ().sensorActivated ();
        OnActivate.Invoke();
    }

    protected virtual void Deactivate ()
    {
        //Debug.Log ($"{transform.parent.name}: {name} Deactivated", this);
        //transform.parent.parent.GetComponent<Enemy_StateMachine>().sensorDeactivated ();
        OnDeactivate.Invoke();
    }
    

    /// <summary>
    /// Checks the sensor's conditions manually.
    /// </summary>
    /// <returns>True if the sensor's conditions are met</returns>
    public virtual bool Ping()
    {
        return false;
    }
}
