using UnityEngine;
using System;
using VContainer;

public class GameOverPresenter : ScreenPresenter
{
    private GameOverView _view => (GameOverView)View;
    private GameManager _gameManager;
    private ScreenManager _screenManager;
    private UIContextRegistry _contextRegistry;
    private AudioManager _audioManager;

    [Inject]
    public void Construct(GameManager gameManager, ScreenManager screenManager, AudioManager audioManager, UIContextRegistry contextRegistry)
    {
        _gameManager = gameManager;
        _screenManager = screenManager;
        _audioManager = audioManager;
        _contextRegistry = contextRegistry;
    }

    public override void Show(bool show)
    {
        base.Show(show);
        _view.Show(show);
    }

    public override void Setup()
    {
        base.Setup();
        
        if (_contextRegistry.TryGet<GameOverUIContext>(out GameOverUIContext context))
        {
            _view.SetSoftCurrencyCollected(context.SoftCurrencyCollected);
            _view.SetHardCurrencyCollected(context.HardCurrencyCollected);
            _view.SetEnemiesKilled(context.EnemiesKilled);
            _view.SetSessionTime(context.GetFormattedSessionTime());
        }
        
        _view.ReplayButton.Initialize();
        _view.ReplayButton.SetInteractable(true);
    }

    public override void OnInstantiated()
    {
        _view.ReplayButton.Clicked += OnReplayButtonClicked;
        _view.HomeButton.Clicked += OnHomeButtonClicked;
    }

    public override void Destroy()
    {
        _view.ReplayButton.Clicked -= OnReplayButtonClicked;
        _view.HomeButton.Clicked -= OnHomeButtonClicked;
        UnityEngine.Object.Destroy(_view.gameObject);
    }

    private void OnReplayButtonClicked(object sender, EventArgs e)
    {
        _audioManager.PlaySoundOneShot(SfxType.GameStart);
        _gameManager.StartNewGame();
    }

    private void OnHomeButtonClicked(object sender, EventArgs e)
    {
        _audioManager.PlaySoundOneShot(SfxType.ButtonClickGeneric);
        _screenManager.ShowScreen(ScreenType.MainMenu);
    }

}
