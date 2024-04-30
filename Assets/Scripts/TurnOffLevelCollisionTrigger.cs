using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLevelCollisionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject levelCollision;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelCollision.SetActive(false);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelCollision.SetActive(true);
        }
    }
}
