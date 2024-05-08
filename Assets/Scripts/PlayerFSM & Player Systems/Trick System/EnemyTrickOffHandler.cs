using System;
using System.Collections;
using Interfaces;
using System.Collections.Generic;
using UnityEngine;
public class EnemyTrickOffHandler : MonoBehaviour
{
    [SerializeField] private float trickDamage;
    [SerializeField] private PlayerBase player;
    [SerializeField] private Vector3 boxSize; // Size of the box for the boxcast

    private void OnEnable()
    {
        ActionEvents.OnTrickPerformed += CheckForAndExecuteTrickOff;
    }

    private void OnDisable()
    {
        ActionEvents.OnTrickPerformed -= CheckForAndExecuteTrickOff;
    }

    private void CheckForAndExecuteTrickOff(Trick trick)
    {
        if (player.stateMachine.currentState == player.airborneState)
        {
            Vector3 boxPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            Collider[] hits = Physics.OverlapBox(boxPos, boxSize / 2, transform.rotation, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (Collider hit in hits)
            {
                IDamageable damageable;
                ITrickOffable trickOffable;

                if (hit.TryGetComponent<IDamageable>(out damageable))
                {
                    damageable.TakeDamage(trickDamage);
                }
                if (hit.TryGetComponent<ITrickOffable>(out trickOffable))
                {
                    trickOffable.TrickOffEvent(player.rb.velocity);
                }
                if (player.rb.velocity.y < 0) player.rb.velocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);
                player.movement.OllieJump();
                break; // Exit the loop after executing the trick off on the first enemy found
                
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), boxSize);
    }
    
}

