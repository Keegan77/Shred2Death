using UnityEngine;
using UnityEngine.InputSystem;

public class RotateWithMouse : MonoBehaviour
{
    public float rotationSpeed = 1f;
    private Vector2 rotation;
    

    private void Update()
    {
        // Get the mouse delta
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Apply the mouse delta to the rotation
        rotation.y += mouseDelta.x * rotationSpeed * Time.deltaTime;
        rotation.x -= mouseDelta.y * rotationSpeed * Time.deltaTime;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f); // Limit the vertical rotation

        // Apply the rotation to the origin transform
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
        
    }
}
