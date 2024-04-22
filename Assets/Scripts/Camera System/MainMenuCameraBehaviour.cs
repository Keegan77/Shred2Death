using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraBehaviour : MonoBehaviour
{
    [SerializeField] private float cameraYRotationSpeed;
    private void Update()
    {
        transform.Rotate(Vector3.up * (cameraYRotationSpeed * Time.deltaTime));
    }
}
