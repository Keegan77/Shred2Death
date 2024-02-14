using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Type_Flying : Enemy_State
{
    protected Enemy_Flying e;

    protected GameObject movePoint;

    [Header("Movement Parameters")]
    public float movementSpeed = 1.0f;
    public float stoppingDistance = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy_Flying>();
    }

    /// <summary>
    /// Used by MoveToPoint to determine if the destination has been reached.
    /// </summary>
    /// <returns>True if the enemy is close enough to a stopping point based on Flying Enemy stopping distance</returns>
    bool isAtPoint ()
    {
        return (Vector3.Distance (transform.position, e.stateMachine.travelPoint) <= stoppingDistance);
    }

    protected IEnumerator MoveToPoint (Vector3 p)
    {
        e.stateMachine.travelPoint = p;

        while (!isAtPoint ())
        {
            yield return null;
        }

        onPointReached ();
    }

    protected virtual void onPointReached ()
    {

    }
}
