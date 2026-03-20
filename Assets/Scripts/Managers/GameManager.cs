using System;
using ArrowFlowGame.Types;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("INPUT")]
    private bool _globalInputEnabled = true;
    public bool GlobalInputEnabled
    {
        get => _globalInputEnabled;
        set => _globalInputEnabled = value;
    }

    private GameState _curGameState = GameState.NOT_STARTED;
    public GameState CurGameState
    {
        get => _curGameState;
        set
        {
            if(value == GameState.STARTED)
                OnGameStarted?.Invoke();

            if(value == GameState.COMPLETED)
                OnLevelComplete?.Invoke();

            if(value == GameState.FAILED)
                OnLevelFailed?.Invoke();
                
            _curGameState = value;
        }
    }

    public event Action OnGameStarted;
    public event Action OnLevelComplete;
    public event Action OnLevelFailed;

    public void Initialize()
    {
        CurGameState = GameState.STARTED;
    }
}