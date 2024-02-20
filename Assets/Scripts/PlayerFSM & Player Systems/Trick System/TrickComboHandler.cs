using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickComboHandler : MonoBehaviour
{
    [Header("Multiplier Settings")]
    
    [Tooltip("The maximum achievable multiplier")]
    [SerializeField] private float maxMultiplier;
    
    [Tooltip("Time before multiplier drops")]  
    [SerializeField] private float multiplierDropTime; 
    
    [Header("Style System Settings")]
    
    [Tooltip("The maximum achievable style level. maxStyleLevel * styleLevelThreshold = maxStylePoints")]
    [SerializeField] private float maxStyleLevel;
    
    [Tooltip("The amount of style points needed to reach the next level")]
    [SerializeField] private float styleLevelThreshold; 
    
    [Tooltip("the time before style points reset to 0")]
    [SerializeField] private float comboDropTime;
    
    float currentStyleLevel;
    float currentStylePoints;
    float currentMultiplier = 1; // we should start this at 1 bc we multiply by this. anything under 1 would be a
                                 // negative multiplier
    
    //deltatime increment values
    private float timeSinceLastTrick; // increments by dletaTime in update and resets to 0 ontrickcompletion
    private float timeSinceMultiplierIncrease; // increments by deltaTime in update , resets to 0 on multiplier increase


    private void Update()
    {
        if (currentStylePoints > 0)
            timeSinceLastTrick += Time.deltaTime;
        
        if (currentMultiplier > 1)
            timeSinceMultiplierIncrease += Time.deltaTime;
        
        if (timeSinceLastTrick > comboDropTime)
        {
            currentStylePoints = 0;
            currentStyleLevel = 0;
            timeSinceLastTrick = 0;
        }
        
        if (timeSinceMultiplierIncrease > multiplierDropTime)
        {
            timeSinceMultiplierIncrease = 0;
            currentMultiplier = 1;
        }
    }

    private void IncrementStylePoints(Trick trick)
    {
        if (currentStyleLevel >= maxStyleLevel) return;
        currentStylePoints += trick.stylePoints * currentMultiplier;
        timeSinceLastTrick = 0;
        
        if (currentStylePoints > styleLevelThreshold)
        {
            currentStyleLevel++;
            
            currentStylePoints = 0;
        }
    }
    

    private void IncrementMultiplier(Trick trick)
    {
        Debug.Log($"Multiplier increased by {trick.multiplierIncrease}");
        Debug.Log($"Current Multiplier: {currentMultiplier}");
        currentMultiplier += trick.multiplierIncrease;
        if (currentMultiplier > maxMultiplier) currentMultiplier = maxMultiplier;
        timeSinceMultiplierIncrease = 0;
    }

    #region Getters

    public float GetStylePoints()
    {
        return currentStylePoints;
    }
    
    public float GetStyleLevel()
    {
        return currentStyleLevel;
    }
    
    public float GetMultiplier()
    {
        return currentMultiplier;
    }
    
    public float GetTimeSinceLastTrick()
    {
        return timeSinceLastTrick;
    }
    
    public float GetTimeSinceMultiplierIncrease()
    {
        return timeSinceMultiplierIncrease;
    }

    #endregion
    #region Trick Event Subscriptions
    private void OnEnable()
    {
        ActionEvents.OnTrickCompletion += IncrementMultiplier; //needs to be called first
        ActionEvents.OnTrickCompletion += IncrementStylePoints;
    }
    private void OnDisable()
    {
        ActionEvents.OnTrickCompletion -= IncrementMultiplier; 
        ActionEvents.OnTrickCompletion -= IncrementStylePoints;
    }
    #endregion
    //TODO: make increment multiplier & increment style points take a float value instead of a trick reference
    //TODO: make the more general above methods subscribe to AddStylePoints as well as ontrickcompletion 
}