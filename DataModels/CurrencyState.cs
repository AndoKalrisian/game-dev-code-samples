using System;

/// <summary>
/// Holds player's persistent currency balances.
/// Manages state only - no business logic or persistence.
/// Registered as singleton in VContainer.
/// </summary>
public class CurrencyState
{
    private int _softCurrency;
    private int _hardCurrency;
    
    /// <summary>
    /// Current soft currency balance
    /// </summary>
    public int SoftCurrency 
    { 
        get => _softCurrency;
        set
        {
            if (_softCurrency != value)
            {
                _softCurrency = value;
                OnSoftCurrencyChanged?.Invoke(_softCurrency);
            }
        }
    }
    
    /// <summary>
    /// Current hard currency balance
    /// </summary>
    public int HardCurrency 
    { 
        get => _hardCurrency;
        set
        {
            if (_hardCurrency != value)
            {
                _hardCurrency = value;
                OnHardCurrencyChanged?.Invoke(_hardCurrency);
            }
        }
    }
    
    /// <summary>
    /// Fired when soft currency changes. Parameter: new total
    /// </summary>
    public event Action<int> OnSoftCurrencyChanged;
    
    /// <summary>
    /// Fired when hard currency changes. Parameter: new total
    /// </summary>
    public event Action<int> OnHardCurrencyChanged;

    public void Initialize(CurrencyConfig config)
    {
        SoftCurrency = config.SoftCurrency;
        HardCurrency = config.HardCurrency;
    }

    /// <summary>
    /// Gets the current balance for a specific currency type.
    /// </summary>
    public int GetCurrency(CurrencyType currencyType)
    {
        return currencyType switch
        {
            CurrencyType.SoftCurrency => SoftCurrency,
            CurrencyType.HardCurrency => HardCurrency,
            _ => 0
        };
    }

    /// <summary>
    /// Sets the balance for a specific currency type.
    /// </summary>
    public void SetCurrency(CurrencyType currencyType, int amount)
    {
        switch (currencyType)
        {
            case CurrencyType.SoftCurrency:
                SoftCurrency = amount;
                break;
            case CurrencyType.HardCurrency:
                HardCurrency = amount;
                break;
        }
    }
}