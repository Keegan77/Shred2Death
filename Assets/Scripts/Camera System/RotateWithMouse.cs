using UnityEngine;
using UnityEngine.InputSystem;

public class RotateWithMouse : MonoBehaviour
{
    public float rotationSpeed = 1f;
    private Vector2 rotation;
    

    private void Update()
    {
        // Get the input delta
        Vector2 lookDelta = InputRouting.Instance.GetLookInput();

        // Apply the input delta to the rotation
        rotation.y += lookDelta.x * rotationSpeed * Time.deltaTime;
        rotation.x -= lookDelta.y * rotationSpeed * Time.deltaTime;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f); // Limit the vertical rotation

        // Apply the rotation to the origin transform
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
        
    }
}
