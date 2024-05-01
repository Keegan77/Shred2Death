using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private bool isDead;
    [SerializeField] private PlayerHUD playerHUD;
    private float timer;
    [SerializeField] float regenHealthCooldown;
    [SerializeField] float healthRegenerationSpeed;
    private PlayerBase player;
    [SerializeField] private Image healthVignette;
    private Color color;
    [SerializeField] private float pulseSpeed;
    [SerializeField] 
    [Range(0, 1)]private float pulseDistanceDivisor;

    private void Start()
    {
        player = FindObjectOfType<PlayerBase>();
        color = Color.white;
        color.a = 0;
        currentHealth = maxHealth;
    }
    
    private void UpdateHealthUI()
    {
        float t = Mathf.InverseLerp(maxHealth, 0, currentHealth);
        float cur = 0;
        cur = Mathf.Lerp(0, .9f, t);
        if (cur > .1f)
        {
            cur += (Mathf.Sin(Time.time * pulseSpeed) + 1) * pulseDistanceDivisor;
        }
        color.a = Mathf.Lerp(color.a, cur, Time.deltaTime * 2);//
        healthVignette.color = color;
    }

    private void Update()
    {
        UpdateHealthUI();
        
        timer += Time.deltaTime;
        
        if (timer > regenHealthCooldown)
        {
            currentHealth += Time.deltaTime * healthRegenerationSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        timer = 0;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }

    public void Die()
    {
        isDead = true;
        playerHUD.subMenuContainer.SetActive(true);
        playerHUD.widgetContainer.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        player.stateMachine.SwitchState(player.deathState);
    }
}