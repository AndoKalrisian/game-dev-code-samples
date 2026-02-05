using UnityEngine;
using System;
using VContainer;

public class HUDPresenter : ScreenPresenter
{
    private HUDView _view => (HUDView)View;
    private PlayerState _player;
    private ShieldState _shield;
    private GameSessionState _sessionData;
    private GameManager _gameManager;
    private ScreenManager _screenManager;

    [Inject]
    public void Construct(PlayerState player, ShieldState shield, GameSessionState sessionData, GameManager gameManager, ScreenManager screenManager)
    {
        _player = player;
        _shield = shield;
        _sessionData = sessionData;
        _gameManager = gameManager;
        _screenManager = screenManager;
    }
    
    public override void Setup()
    {
        base.Setup();
        
        _view.SetHealthValue(_player.GetHealthPercentage(), _player.CurrentHealth);
        _view.SetShieldValue(_shield.GetShieldPercentage(), _shield.CurrentShieldStrength);
        _view.SetSoftCurrencyCollected(_sessionData.SoftCurrencyCollected);
        _view.SetHardCurrencyCollected(_sessionData.HardCurrencyCollected);
        _view.SetGameModifierCurrencyCollected(_sessionData.GameModifierCurrencyCollected);
        _view.SetLevelNumber(_gameManager.CurrentLevel.LevelNumber);

    }

    public override void OnInstantiated()
    {
        base.OnInstantiated();
        _player.OnHealthChanged += OnHealthChangedHandler;
        _shield.OnShieldStrengthChanged += OnShieldStrengthChangedHandler;
        _sessionData.OnSoftCurrencyCollected += OnSoftCurrencyChangedHandler;
        _sessionData.OnHardCurrencyCollected += OnHardCurrencyChangedHandler;
        _sessionData.OnGameModifierCurrencyCollected += OnGameModifierCurrencyChangedHandler;
        _gameManager.OnLevelNumberChanged += OnLevelNumberChangedHandler;
        _gameManager.OnTimeUntilBossChanged += OnTimeUntilBossChangedHandler;
        _gameManager.OnLevelProgressChanged += OnLevelProgressChangedHandler;
        _view.pauseButton.Clicked += OnPauseClicked;
        _view.gameModifierButton.Clicked += (s, e) => 
        {
            _gameManager.PauseGame();
            _screenManager.ShowScreen(ScreenType.GameModifierOverlay);
        };

    }

    public override void Destroy()
    {
        base.Destroy();
        
        if (_player != null)
        {
            _player.OnHealthChanged -= OnHealthChangedHandler;
        }
        
        if (_sessionData != null)
        {
            _sessionData.OnSoftCurrencyCollected -= OnSoftCurrencyChangedHandler;
        }
        
        if (_gameManager != null)
        {
            _gameManager.OnLevelNumberChanged -= OnLevelNumberChangedHandler;
            _gameManager.OnTimeUntilBossChanged -= OnTimeUntilBossChangedHandler;
            _gameManager.OnLevelProgressChanged -= OnLevelProgressChangedHandler;
        }

        _view.pauseButton.Clicked -= OnPauseClicked;

        UnityEngine.Object.Destroy(_view.gameObject);
    }

    private void OnHealthChangedHandler(float healthPercentage, float currentHealth)
    {
        _view.SetHealthValue(healthPercentage, currentHealth);
    }

    private void OnShieldStrengthChangedHandler(float shieldPercentage, float currentShieldStrength)
    {
        _view.SetShieldValue(shieldPercentage, currentShieldStrength);
    }
    
    private void OnSoftCurrencyChangedHandler(int amount)
    {
        _view.SetSoftCurrencyCollected(amount);
    }

    private void OnHardCurrencyChangedHandler(int amount)
    {
        _view.SetHardCurrencyCollected(amount);
    }
    
    private void OnGameModifierCurrencyChangedHandler(int amount)
    {
        _view.SetGameModifierCurrencyCollected(amount);
    }
    
    private void OnLevelNumberChangedHandler(int levelNumber)
    {
        _view.SetLevelNumber(levelNumber);
    }
    
    private void OnTimeUntilBossChangedHandler(float timeRemaining)
    {
        _view.SetTimer(timeRemaining);
    }
    
    private void OnLevelProgressChangedHandler(float progress)
    {
        _view.SetLevelProgress(progress);
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        _gameManager.PauseGame();
        _screenManager.ShowScreen(ScreenType.PauseGameOverlay);
    }
}