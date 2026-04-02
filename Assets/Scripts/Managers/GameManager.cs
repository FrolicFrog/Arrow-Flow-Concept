using System;
using ArrowFlowGame.Types;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    private float _targetFPS = 28f;
    private float _minScale = 0.85f;
    private float _maxScale = 1.2f;
    private float _adjustSpeed = 0.2f;

    private float _deltaTime;
    private UniversalRenderPipelineAsset _urpAsset;

    public void Initialize()
    {
        _urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        CurGameState = GameState.STARTED;
        AnalyticsManager.LevelStarted(LevelManager.Instance.CurrentLevelNumber);
    }

    private void Update()
    {
        HandleDynamicResolution();
    }

    private void HandleDynamicResolution()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;

        if(_urpAsset == null) return;

        float scale = _urpAsset.renderScale;

        if(fps < _targetFPS)
        {
            scale -= _adjustSpeed * Time.deltaTime;
        }
        else if(fps > _targetFPS + 5f)
        {
            scale += _adjustSpeed * Time.deltaTime;
        }

        scale = Mathf.Clamp(scale, _minScale, _maxScale);
        _urpAsset.renderScale = scale;
    }
}