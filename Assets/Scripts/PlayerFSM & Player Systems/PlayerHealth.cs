using UnityEngine;

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

    private void Start()
    {
        player = FindObjectOfType<PlayerBase>();
        currentHealth = maxHealth;
    }
    
    private void UpdateHealthUI()
    {
        playerHUD.stats.healthBar.currentValue = Mathf.Lerp(playerHUD.stats.healthBar.currentValue, currentHealth / maxHealth, Time.deltaTime * 5);
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
        //playerHUD.openMenu(playerHUD.menuGameOver);
        Time.timeScale = 0;
    }
}