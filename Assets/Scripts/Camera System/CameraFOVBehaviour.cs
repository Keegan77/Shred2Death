using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFOVBehaviour : MonoBehaviour
{
    [SerializeField] PlayerBase player;
    [SerializeField] private Camera camera;
    private Rigidbody rb;
    [SerializeField ]float baseFOV;
    [SerializeField] private float FOVChangeSpeed;
    private float currentFOV;
    private float magnitude;
    bool dynamicFOV;
    
    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        currentFOV = baseFOV;
        dynamicFOV = true;
    }

    private void OnEnable()
    {
        ActionEvents.StartedGameplayCutscene += delegate { dynamicFOV = false; };
        ActionEvents.EndedGameplayCutscene += delegate { dynamicFOV = true; };
    }

    private void OnDisable()
    {
        ActionEvents.StartedGameplayCutscene -= delegate { dynamicFOV = false; };
        ActionEvents.EndedGameplayCutscene -= delegate { dynamicFOV = true; };
    }

    private void Update()
    {
        if (rb == null) return;
        if (!dynamicFOV) return;
        magnitude = Mathf.Lerp(magnitude, rb.velocity.magnitude / 1.5f, FOVChangeSpeed * Time.unscaledDeltaTime);
        //Debug.Log(rb.velocity.magnitude);
        if (magnitude != 0)
        {
            currentFOV = baseFOV + magnitude / 3;
            currentFOV = Mathf.Clamp(currentFOV, player.playerData.minFOV, player.playerData.maxFOV);
            camera.fieldOfView = currentFOV;
        }
        
    }
}
