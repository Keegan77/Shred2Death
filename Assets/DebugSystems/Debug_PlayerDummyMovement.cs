using System.Collections;
using System.Collections.Generic;
using Interfaces;
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
    public bool controlsEnabled = true;
    private bool cameraKey = true;
    private bool cameraKeyPrev = false;

    GameObject cameraObject;
    GameObject cameraPivot;
    GameObject cameraAnchor;

    [SerializeField] CursorLockMode cameraModeActive;
    [SerializeField] CursorLockMode cameraModePause;

    public PlayerHUD hud;

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


        Cursor.lockState = cameraModeActive;
    }

    private void Start ()
    {
        cameraObject = Camera.main.gameObject;

        if (useCamera)
        {
            cameraObject.transform.SetParent (cameraAnchor.transform, false);
            cameraObject.transform.position = cameraAnchor.transform.position;
            cameraObject.transform.rotation = cameraAnchor.transform.rotation;
        }
        
    }

    //sets the camera key to sync with the menu better.
    public void pauseGame(bool c)
    {
        hud.ToggleGamePaused ();

        if (c) Cursor.lockState = cameraModePause;
        else Cursor.lockState = cameraModeActive;

        cameraKey = !c;
    }

    private void OnEnable ()
    {
        InputRouting.Instance.input.UI.Pause.performed += ctx => pauseGame(cameraKey);
    }
    private void OnDisable ()
    {
        InputRouting.Instance.input.UI.Pause.performed -= ctx => pauseGame (cameraKey);
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlsEnabled) return;

        bool clicked = InputRouting.Instance.GetBoostInput ();
        Vector3 rot = cameraRotation * cameraSensitivity * Time.deltaTime;


        //If the player has clicked
        if (clicked && !cameraKeyPrev)
        {
            cameraKey = hud.gameObject.activeSelf;
        }

        if (cameraKey)
        {
            rotationTrack += rot;
            transform.rotation = Quaternion.Euler (rotationTrack);
        }
        


        cameraKeyPrev = clicked;
    }

    private void FixedUpdate ()
    {
        if (!controlsEnabled) return;


        Vector2 mov = InputRouting.Instance.GetMoveInput ();

        movement = new Vector3 (mov.x, movementVertical, mov.y);

        rb.velocity = transform.rotation * (movement.normalized * movementSpeed);
    }

    public void TakeDamage (float damage)
    {
        Debug.Log ("Robot Hit by bullet");
    }
}
