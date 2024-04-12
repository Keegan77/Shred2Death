using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RagdollHitbox : MonoBehaviour, IDamageable
{
    [SerializeField] Enemy e;

    private void Start ()
    {
        Debug.Log (e);
    }

    public void TakeDamage (float damage)
    {
        e.TakeDamage (damage);
    }
}
