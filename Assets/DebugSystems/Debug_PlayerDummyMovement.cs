using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Place this in a scene to gain movement controls.
/// </summary>
/// 
[SelectionBase]
public class Debug_PlayerDummyMovement : MonoBehaviour
{
    public float movementSpeed = 1;

    Vector3 movement = Vector3.zero;
    float movementVertical 
    { 
        get 
        {
            float temp = 0;
            if ( InputRouting.Instance.GetDriftInput () ) temp -= 1;
            if ( InputRouting.Instance.GetJumpInput () ) temp += 1;
            return temp;
        }
        set { movementVertical = value; }
    }

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate ()
    {
        Vector2 mov = InputRouting.Instance.GetMoveInput ();

        movement = new Vector3 (mov.x, movementVertical, mov.y);

        rb.velocity = transform.rotation * (movement.normalized * movementSpeed);
    }
}
