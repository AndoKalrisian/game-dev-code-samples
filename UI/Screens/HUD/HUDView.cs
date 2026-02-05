using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDView : ScreenView
{
    [Header("UI References")]
    [SerializeField] private TextView softCurrencyCollectedText;
    [SerializeField] private TextView softCurrencyCollectedValueText;
    [SerializeField] private TextView hardCurrencyCollectedText;
    [SerializeField] private TextView hardCurrencyCollectedValueText;
    [SerializeField] private TextView gameModifierCurrencyCollectedText;
    [SerializeField] private TextView gameModifierCurrencyCollectedValueText;
    [SerializeField] private TextView healthText;
    [SerializeField] private TextView healthValueText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextView shieldText;
    [SerializeField] private TextView shieldValueText;
    [SerializeField] private Image shieldBarFill;
    [SerializeField] private TextView levelNumberText;
    [SerializeField] private TextView timerText;
    [SerializeField] private Image levelProgressBar;
    [SerializeField] public ButtonView pauseButton;
    [SerializeField] public ButtonView gameModifierButton;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show(bool show)
    {
        base.Show(show);
        healthText.Show(show);
        healthValueText.Show(show);
        healthBarFill.gameObject.SetActive(show);
        shieldText.Show(show);
        shieldValueText.Show(show);
        shieldBarFill.gameObject.SetActive(show);
        softCurrencyCollectedText.Show(show);
        softCurrencyCollectedValueText.Show(show);
        pauseButton.Show(show);
        gameModifierButton.Show(show);
        hardCurrencyCollectedText.Show(show);
        hardCurrencyCollectedValueText.Show(show);
        gameModifierCurrencyCollectedText.Show(show);
        gameModifierCurrencyCollectedValueText.Show(show);
        
        if (levelNumberText != null)
            levelNumberText.Show(show);
        if (timerText != null)
            timerText.Show(show);
        if (levelProgressBar != null)
            levelProgressBar.gameObject.SetActive(show);
    }

    public void SetHealthValue(float healthPercentage, float currentHealth)
    {
        healthValueText.Text = Math.Round(currentHealth, 1).ToString();
        healthBarFill.fillAmount = healthPercentage;
    }

    public void SetShieldValue(float shieldPercentage, float currentShieldStrength)
    {
        shieldValueText.Text = Math.Round(currentShieldStrength, 2).ToString();
        shieldBarFill.fillAmount = shieldPercentage;
    }

    public void SetSoftCurrencyCollected(int amount)
    {
        softCurrencyCollectedValueText.Text = amount.ToString();
    }

    public void SetHardCurrencyCollected(int amount)
    {
        hardCurrencyCollectedValueText.Text = amount.ToString();
    }

    public void SetGameModifierCurrencyCollected(int amount)
    {
        gameModifierCurrencyCollectedValueText.Text = amount.ToString();
    }
    
    public void SetLevelNumber(int levelNumber)
    {
        if (levelNumberText != null)
            levelNumberText.Text = $"Level {levelNumber}";
    }
    
    public void SetTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.Text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    public void SetLevelProgress(float progress)
    {
        if (levelProgressBar != null)
            levelProgressBar.fillAmount = progress;
    }
}