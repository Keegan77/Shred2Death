using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputRouting : MonoBehaviour // Singleton which inherits it's DoNotDestroyOnLoad property from it's parent
{                                         // object, which is instantiated in the Bootstrapper class.
    public static InputRouting Instance;
    
    public Input input { get; private set; }
    private Vector2 moveInput;
    private Vector2 bumperInput;
    private Vector2 lookInput;
    private bool driftHeld;
    private bool boostHeld;
    private bool jumpHeld;
    private bool fireHeld;
    private void Awake()
    {
        if (Instance == null)
        {
            input = new Input();
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        lookInput = input.Player.Look.ReadValue<Vector2>();
        bumperInput = input.Player.AirRotation.ReadValue<Vector2>();
    }

    public Vector2 GetBumperInput()
    {
        return bumperInput;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public Vector2 GetLookInput()
    {
        return lookInput;
    }
    
    public bool GetDriftInput(bool alsoCheckForMoveInput = false)
    {
        if (alsoCheckForMoveInput)
        {
            if (moveInput.x != 0)
                return driftHeld;
            else return false;
        }
        else return driftHeld;
    }
    
    public bool GetBoostInput()
    {
        return boostHeld;
    }
    
    public bool GetFireHeld()
    {
        return fireHeld;
    }
    
    public bool GetJumpInput()
    {
        return jumpHeld;
    }
    private void OnEnable()
    {
        input.Enable();
        //drift input should be true if the value is 1
        input.Player.Drift.performed += ctx => driftHeld = true;
        input.Player.Drift.canceled += ctx => driftHeld = false;
        
        input.Player.Jump.performed += ctx => jumpHeld = true;
        input.Player.Jump.canceled += ctx => jumpHeld = false;
        
        input.Player.Boost.performed += ctx => boostHeld = true;
        input.Player.Boost.canceled += ctx => boostHeld = false;
            
        input.Player.Fire.performed += ctx => fireHeld = true;
        input.Player.Fire.canceled += ctx => fireHeld = false;
    }

    private void OnDisable()
    {
        input.Player.Drift.performed -= ctx => driftHeld = true;
        input.Player.Drift.canceled -= ctx => driftHeld = false;
        
        input.Player.Jump.performed -= ctx => jumpHeld = true;
        input.Player.Jump.canceled -= ctx => jumpHeld = false;
        
        input.Player.Boost.performed -= ctx => boostHeld = true;
        input.Player.Boost.canceled -= ctx => boostHeld = false;
        
        input.Player.Fire.performed -= ctx => fireHeld = true;
        input.Player.Fire.canceled -= ctx => fireHeld = false;
        input.Disable();
    }
}
