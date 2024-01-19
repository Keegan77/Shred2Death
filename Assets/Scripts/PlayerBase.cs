using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    // Components
    private Rigidbody rb;
    private ConstantForce extraGravity; // Extra gravity to make the player fall faster. Unity base gravity,
                                        // in most cases, Unity's base gravity value feels very slow and floaty,
                                        // and this will counteract that.
    //Input
    private Input input;
    // Movement values
    [Header("Movement Values")]
    [SerializeField] float baseMovementSpeed;

    private void Awake()
    {
        input = new Input();
        rb = GetComponent<Rigidbody>();
        extraGravity = GetComponent<ConstantForce>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void FixedUpdate()
    {
        
    }
}
