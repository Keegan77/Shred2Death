using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedNeedleBehaviour : MonoBehaviour
{
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;
    private float targetRotation;
    [SerializeField] private PlayerBase player;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float needleSpeed;
    [SerializeField] private float addedRandomnessThreshold;
    private void Update()
    {
        float speed = player.rb.velocity.magnitude;
        targetRotation = Mathf.Lerp(minRotation, maxRotation, speed / maxSpeed);

        if (speed > 5)
        {
            targetRotation += Random.Range(-20, 20);
        }
        
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * needleSpeed);
    }
}
