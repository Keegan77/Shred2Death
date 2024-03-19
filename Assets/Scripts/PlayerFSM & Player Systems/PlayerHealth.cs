using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private bool isDead;
    [SerializeField] private PlayerHUD playerHUD;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    private void UpdateHealthUI()
    {
        playerHUD.stats.healthBar.currentValue = Mathf.Lerp(playerHUD.stats.healthBar.currentValue, currentHealth / maxHealth, Time.deltaTime * 5);
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        //death code
    }
}