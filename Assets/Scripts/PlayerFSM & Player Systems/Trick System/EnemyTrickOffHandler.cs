using System;
using System.Collections;
using Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrickOffHandler : MonoBehaviour
{
    Stack<GameObject> enemiesInTriggerStack = new Stack<GameObject>();
    [SerializeField] private float trickDamage;
    [SerializeField] PlayerBase player;

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
        if (enemiesInTriggerStack.Count > 0 && player.stateMachine.currentState == player.airborneState)
        {
            // get the top enemy on the stack
            GameObject enemy = enemiesInTriggerStack.Peek();

            IDamageable damageable;
            ITrickOffable trickOffable;
            
            if (enemy.gameObject == null) return;
            if (enemy.TryGetComponent<IDamageable>(out damageable))
            {
                damageable.TakeDamage(trickDamage);
            }
            if (enemy.TryGetComponent<ITrickOffable>(out trickOffable))
            {
                trickOffable.TrickOffEvent(player.rb.velocity);
            }
            if (player.rb.velocity.y < 0) player.rb.velocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);
            player.movement.OllieJump();
                // sending in player velocity so demons can potentially
                //calculate a knockback force based on player velocity
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemiesInTriggerStack.Push(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemiesInTriggerStack.Pop();
        }
    }
}
