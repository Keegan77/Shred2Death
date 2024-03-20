using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCollisionMover : MonoBehaviour
{
    [SerializeField] private PlayerBase player;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y - 30, transform.position.z);
    }
}
