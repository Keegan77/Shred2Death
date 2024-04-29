using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
/// <summary>
/// Used for managing the current state of our game. This class is a singleton, and will persist between scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public enum GameState
    {
        MainMenu,
        InGame,
        Paused,
        GameOver
    }

    private GameState currentState = GameState.InGame;
    
    #region Unity Event Functions

    private void Awake() //Ran once when the application is opened, and never again
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Getters and Setters

    public void SetGameState(GameState newState)
    {
        currentState = newState;
    }
    
    public GameState GetGameState()
    {
        return currentState;
    }

    #endregion

}
