using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkateboardSlopePositioner : MonoBehaviour
{
    [SerializeField] private PlayerBase player;
    [SerializeField] private float ZOffset;
    [SerializeField] private float verticalYOffset, horizontalYOffset;
    private Vector3 startingPos;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float rayNormalDistance; //1.3
    [SerializeField] private float rayHalfPipeDist;
    private float rayDistance;
    private float skateboardGrindPos = -.76f;
    float YOffset;
    private bool justHitGround;
    private bool offsetOverridden;

    //[SerializeField] private float raycastZOffsetFromOrigin;

    private void Awake()
    {
        startingPos = transform.localPosition;
    }

    public void OverrideYOffset(float newOffset)
    {
        // lerp y offset
        StartCoroutine(LerpYOffset(newOffset));
        offsetOverridden = true;
    }
    
    private IEnumerator LerpYOffset(float newOffset)
    {
        float t = 0;
        float startOffset = YOffset;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            YOffset = Mathf.Lerp(startOffset, newOffset, t);
            yield return null;
        }
    }
    
    public void ResetOffsetOverride()
    {
        StartCoroutine(LerpYOffset(0));
        offsetOverridden = false;
    }
    
    private void FixedUpdate()
    {
        SetRayDistances();
        
        
        if (player.stateMachine.currentState != player.halfPipeState && !offsetOverridden)
        {
            
            float percentageToNinety = Mathf.Abs(player.GetOrientationWithDownward() - 90) / 90;
            YOffset = Mathf.Lerp(verticalYOffset, horizontalYOffset, percentageToNinety); // YOffset needs to be dynamically changed based on the player's orientation
        }
        else if (!offsetOverridden)
        {
            YOffset = horizontalYOffset;
        }
        
        if (Physics.Raycast(raycastOrigin.position, -raycastOrigin.up, out RaycastHit hit, rayDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            if (!justHitGround)
            {
                ActionEvents.PlayerSFXOneShot?.Invoke(SFXContainerSingleton.Instance.landingSounds[UnityEngine.Random.Range(0, SFXContainerSingleton.Instance.landingSounds.Count)], 0);
            }
            justHitGround = true;
            Vector3 localOffset = transform.TransformDirection(new Vector3(0, YOffset, ZOffset));
            transform.position = hit.point + localOffset;
            if (hit.collider.CompareTag("BurnDamage"))
            {
                player.movement.DoBurnForce(hit.point, 10, keepHozForces:true);
            }
        }
        else
        {
            justHitGround = false;
            transform.localPosition = startingPos;
        }
        if (player.stateMachine.currentState == player.grindState)
        {
            transform.localPosition = new Vector3(startingPos.x, skateboardGrindPos, startingPos.z);
        }
    }

    private void SetRayDistances()
    {
        if (player.stateMachine.currentState != player.halfPipeState)
        {
            rayDistance = rayNormalDistance;
        }
        else
        {
            rayDistance = rayHalfPipeDist;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position - raycastOrigin.up * rayDistance);
    }
}
