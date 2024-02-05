using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVBehaviour : MonoBehaviour
{
    [SerializeField] GameObject player;
    private Camera camera;
    private Rigidbody rb;
    float defaultFOV = 40;
    [SerializeField] private float FOVChangeSpeed;
    private float currentFOV;
    private float magnitude;
    
    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        currentFOV = defaultFOV;
        camera = GetComponent<Camera>();
    }

    private void Update() //TODO: Add a limit for the maximum FOV 
    {
        magnitude = Mathf.Lerp(magnitude, rb.velocity.magnitude, FOVChangeSpeed * Time.deltaTime);
        //Debug.Log(rb.velocity.magnitude);
        if (magnitude != 0)
        {
            currentFOV = defaultFOV + magnitude / 3;
            camera.fieldOfView = currentFOV;
        }
        
    }
}
