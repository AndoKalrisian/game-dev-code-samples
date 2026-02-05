using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyBarView : ElementView
{
    [Header("UI References")]
    [SerializeField] private TextView SoftCurrencyValueText;
    [SerializeField] private ImageView SoftCurrencyIconImage;
    [SerializeField] private TextView HardCurrencyValueText;
    [SerializeField] private ImageView HardCurrencyIconImage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show(bool show)
    {
        base.Show(show);
        SoftCurrencyValueText.Show(show);
        SoftCurrencyIconImage.Show(show);
        HardCurrencyValueText.Show(show);
        HardCurrencyIconImage.Show(show);
    }

    public void SetSoftCurrencyAmount(int amount)
    {
        SoftCurrencyValueText.Text = amount.ToString();
    }
    
    public void SetHardCurrencyAmount(int amount)
    {
        HardCurrencyValueText.Text = amount.ToString();
    }
}