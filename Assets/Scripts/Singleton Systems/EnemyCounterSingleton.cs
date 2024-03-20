using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EnemyCounterSingleton : MonoBehaviour
{
    public static EnemyCounterSingleton Instance;
    public int enemyCount;
    public PlayerHUD playerHUD;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    
    private void Start()
    {
        playerHUD = FindObjectOfType<PlayerHUD>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerHUD = FindObjectOfType<PlayerHUD>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (playerHUD != null) UpdateEnemyHUD();
    }

    private void UpdateEnemyHUD()
    {
        playerHUD.stats.enemyText.setTextCount(enemyCount);
    }
    
}
