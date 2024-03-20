using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCollisionMover : MonoBehaviour
{
    [SerializeField] private PlayerBase player;
    [SerializeField] private float offset;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y + offset, transform.position.z);
    }
}
