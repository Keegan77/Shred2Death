using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuouslyRotateUI : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    void Update()
    {
        transform.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed));
    }
}
