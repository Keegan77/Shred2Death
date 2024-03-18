using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Place this in a scene to gain movement controls.
/// </summary>
/// 
[SelectionBase]
public class Debug_PlayerDummyMovement : MonoBehaviour, IDamageable
{
    public float movementSpeed = 1;
    public float cameraSensitivity = 1;
    public bool useCamera = true;
    private bool cameraKey = true;
    private bool cameraKeyPrev = false;

    public GameObject cameraObject;
    GameObject cameraPivot;
    GameObject cameraAnchor;

    Vector3 rotationTrack = Vector3.zero;


    Vector3 movement = Vector3.zero;
    Vector3 cameraRotation { 
        get
        {
            Vector2 input = InputRouting.Instance.GetLookInput ();

            return new Vector3 (-input.y, input.x, 0);
        }
        set
        {
            cameraRotation = value;
        }
    }
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
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraPivot = transform.Find ("CameraPivot").gameObject;
        cameraAnchor = transform.Find ("CameraPivot/CameraAnchor").gameObject;

        rotationTrack = transform.rotation.eulerAngles;
    }

    private void Start ()
    {
        if (useCamera)
        {
            cameraObject.transform.SetParent (cameraAnchor.transform, false);
            cameraObject.transform.position = cameraAnchor.transform.position;
            cameraObject.transform.rotation = cameraAnchor.transform.rotation;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        bool clicked = InputRouting.Instance.GetBoostInput ();
        Vector3 rot = cameraRotation * cameraSensitivity * Time.deltaTime;
        

        //If the player has clicked
        if (clicked && !cameraKeyPrev ) 
        {
            //Debug.Log ("Mouse clicked");

            //Unparent the camera from the anchor
            if (cameraKey)
            {
                //cameraObject.transform.SetParent (null, true);
                Cursor.lockState = CursorLockMode.Locked;
            }

            //parent the camera to the anchor
            else
            {

                Cursor.lockState = CursorLockMode.None;
            }

            cameraKey = !cameraKey;
        }

        if(!cameraKey)
        {
            rotationTrack += rot;
            transform.rotation = Quaternion.Euler (rotationTrack);
        }
        


        cameraKeyPrev = clicked;
    }

    private void FixedUpdate ()
    {
        Vector2 mov = InputRouting.Instance.GetMoveInput ();

        movement = new Vector3 (mov.x, movementVertical, mov.y);

        rb.velocity = transform.rotation * (movement.normalized * movementSpeed);
    }

    public void TakeDamage (float damage)
    {
        Debug.Log ("Robot Hit by bullet");
    }
}
