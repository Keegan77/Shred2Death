using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputRouting : MonoBehaviour // Singleton which inherits it's DoNotDestroyOnLoad property from it's parent
{                                         // object, which is instantiated in the Bootstrapper class.
    public static InputRouting Instance;
    
    public Input input { get; private set; }
    private Vector2 moveInput;
    private bool driftHeld;
    private bool boostHeld;
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
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
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
    private void OnEnable()
    {
        input.Enable();
        //drift input should be true if the value is 1
        input.Player.Drift.performed += ctx => driftHeld = true;
        input.Player.Drift.canceled += ctx => driftHeld = false;
        
        input.Player.Boost.performed += ctx => boostHeld = true;
        input.Player.Boost.canceled += ctx => boostHeld = false;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Drift.performed -= ctx => driftHeld = true;
        input.Player.Drift.canceled -= ctx => driftHeld = false;
        
        input.Player.Boost.performed -= ctx => boostHeld = true;
        input.Player.Boost.canceled -= ctx => boostHeld = false;
    }
}
