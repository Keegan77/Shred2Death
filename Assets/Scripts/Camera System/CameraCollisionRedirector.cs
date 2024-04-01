using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionRedirector : MonoBehaviour
{
    [SerializeField] private Transform orbitalPoint;
    [SerializeField] private Transform ADSPoint;
    private CameraADSController cameraADSController;
    private Vector3 cameraStartingLocalPos;
    private float cameraDistanceFromOrbital;
    private float adsDistanceFromOrbital;
    private float currentCamDistance;
    int layerToIgnore; 
    int layerMask;
    private void Start()
    {
        cameraDistanceFromOrbital = Vector3.Distance(orbitalPoint.position, transform.position);
        adsDistanceFromOrbital    = Vector3.Distance(orbitalPoint.position, ADSPoint.position);
        
        currentCamDistance = cameraDistanceFromOrbital;
        layerToIgnore = LayerMask.NameToLayer("BowlMesh");
        layerMask = ~(1 << layerToIgnore);
        cameraADSController = GetComponent<CameraADSController>();
        cameraStartingLocalPos = transform.localPosition;
    }
    
    private void Update()
    {
        DetectCollision();
        
        if (cameraADSController.ADSEnabled)
        {
            currentCamDistance = adsDistanceFromOrbital;
        }
        else currentCamDistance = cameraDistanceFromOrbital;
    }

    private void DetectCollision()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(orbitalPoint.position, transform.position - orbitalPoint.position, out hit, currentCamDistance, layerMask))
        {
            transform.position = hit.point;
        }
        else
        {
            if (cameraADSController.ADSEnabled)
            {
                transform.position = ADSPoint.position;
            }
            else transform.localPosition = cameraStartingLocalPos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(orbitalPoint.position, transform.position - orbitalPoint.position);
    }
}
