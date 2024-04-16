using UnityEngine;

public class SelectionTrailBehaviour : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radius; // Radius of the circle

    private float angle = 0.0f; // Current angle in radians

    private void Update()
    {
        // Calculate the new position
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, transform.position.y, z);

        // Update the angle for the next frame
        angle += rotationSpeed * Time.deltaTime;
    }
}
