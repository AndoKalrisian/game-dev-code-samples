using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverView : ScreenView
{
    [Header("UI References")]
    [SerializeField] private TextView TitleText;
    [SerializeField] private TextView SoftCurrencyCollectedText;
    [SerializeField] private TextView SoftCurrencyCollectedValueText;
    [SerializeField] private TextView HardCurrencyCollectedText;
    [SerializeField] private TextView HardCurrencyCollectedValueText;
    [SerializeField] private TextView EnemiesKilledText;
    [SerializeField] private TextView EnemiesKilledValueText;
    [SerializeField] private TextView SessionTimeText;
    [SerializeField] private TextView SessionTimeValueText;
    [SerializeField] public ButtonView ReplayButton;
    [SerializeField] public ButtonView HomeButton;


    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show(bool show)
    {
        base.Show(show);

        TitleText.Show(show);
        SoftCurrencyCollectedText.Show(show);
        SoftCurrencyCollectedValueText.Show(show);
        HardCurrencyCollectedText.Show(show);
        HardCurrencyCollectedValueText.Show(show);
        EnemiesKilledText.Show(show);
        EnemiesKilledValueText.Show(show);
        SessionTimeText.Show(show);
        SessionTimeValueText.Show(show);
        ReplayButton.Show(show);
        HomeButton.Show(show);
    }

    public void SetSoftCurrencyCollected(int amount)
    {
        SoftCurrencyCollectedValueText.Text = $"{amount}";
    }

    public void SetHardCurrencyCollected(int amount)
    {
        HardCurrencyCollectedValueText.Text = $"{amount}";
    }

    public void SetEnemiesKilled(int amount)
    {
        EnemiesKilledValueText.Text = $"{amount}";
    }

    public void SetSessionTime(string formattedTime)
    {
        SessionTimeValueText.Text = formattedTime;
    }
}