using System;
using UnityEngine;
using VContainer;

/// <summary>
/// Stateless service for currency operations.
/// Handles transactions, affordability checks, and validation.
/// Registered as singleton in VContainer's GameLifetimeScope.
/// </summary>
public class CurrencyService
{
    public readonly CurrencyState CurrencyState;
    
    /// <summary>
    /// Fired when any currency transaction occurs. 
    /// Parameters: (currencyType, amountChanged, newTotal)
    /// </summary>
    public event Action<CurrencyType, int, int> OnCurrencyTransaction;

    [Inject]
    public CurrencyService(CurrencyState currencyState)
    {
        CurrencyState = currencyState;
    }

    /// <summary>
    /// Checks if player can afford a purchase with the specified currency type.
    /// </summary>
    /// <param name="cost">Cost of the item</param>
    /// <param name="currencyType">Type of currency required</param>
    /// <returns>True if player has enough currency</returns>
    public bool CanAfford(int cost, CurrencyType currencyType)
    {
        if (cost < 0)
        {
            Debug.LogWarning($"CurrencyService: Invalid cost: {cost}");
            return false;
        }

        return CurrencyState.GetCurrency(currencyType) >= cost;
    }

    /// <summary>
    /// Attempts to spend currency. Only succeeds if player has sufficient funds.
    /// </summary>
    /// <param name="amount">Amount to spend (must be positive)</param>
    /// <param name="currencyType">Type of currency to spend</param>
    /// <returns>True if transaction succeeded, false if insufficient funds</returns>
    public bool TrySpend(int amount, CurrencyType currencyType)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"CurrencyService: Invalid spend amount: {amount}");
            return false;
        }

        int currentAmount = CurrencyState.GetCurrency(currencyType);
        
        if (currentAmount < amount)
        {
            Debug.LogWarning($"CurrencyService: Insufficient {currencyType}. Need {amount}, have {currentAmount}");
            return false;
        }

        int newAmount = currentAmount - amount;
        CurrencyState.SetCurrency(currencyType, newAmount);
        
        OnCurrencyTransaction?.Invoke(currencyType, -amount, newAmount);
        
        return true;
    }

    /// <summary>
    /// Deducts currency without validation. Used by services that already validated affordability.
    /// </summary>
    /// <param name="amount">Amount to deduct</param>
    /// <param name="currencyType">Type of currency to deduct</param>
    public void DeductCurrency(int amount, CurrencyType currencyType)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"CurrencyService: Invalid deduct amount: {amount}");
            return;
        }

        int currentAmount = CurrencyState.GetCurrency(currencyType);
        int newAmount = currentAmount - amount;
        
        CurrencyState.SetCurrency(currencyType, newAmount);
        
        OnCurrencyTransaction?.Invoke(currencyType, -amount, newAmount);
    }

    /// <summary>
    /// Adds currency to the player's wallet. Used when collecting currency drops or rewards.
    /// </summary>
    /// <param name="amount">Amount to add (must be positive)</param>
    /// <param name="currencyType">Type of currency to add</param>
    public void AddCurrency(int amount, CurrencyType currencyType)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"CurrencyService: Invalid add amount: {amount}");
            return;
        }

        int currentAmount = CurrencyState.GetCurrency(currencyType);
        int newAmount = currentAmount + amount;
        
        CurrencyState.SetCurrency(currencyType, newAmount);
        
        OnCurrencyTransaction?.Invoke(currencyType, amount, newAmount);
    }

    /// <summary>
    /// Gets the current balance for a specific currency type.
    /// </summary>
    public int GetCurrency(CurrencyType currencyType)
    {
        return CurrencyState.GetCurrency(currencyType);
    }
}

