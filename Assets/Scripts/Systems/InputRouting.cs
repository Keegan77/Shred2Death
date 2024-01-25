using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputRouting : MonoBehaviour // Singleton which inherits it's DoNotDestroyOnLoad property from it's parent
{                                         // object, which is instantiated in the Bootstrapper class.
    public static InputRouting Instance;
    
    public Input input { get; private set; }
    private Vector2 moveInput;
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
    
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
