using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Manages permanent player upgrades (weapons, shields, health).
/// Handles purchase validation, state tracking, and stat application.
/// </summary>
public class UpgradesService
{
    private readonly PlayerState _player;
    private readonly WeaponState _weapon;
    private readonly ShieldState _shield;
    private readonly PlayerReticle _playerReticle;
    private readonly EnemySpawner _enemySpawner;
    
    /// <summary>
    /// Runtime state for all upgrades (current level, costs, values).
    /// </summary>
    private Dictionary<UpgradeId, UpgradeItemState> _upgradeStates;

    [Inject]
    public UpgradesService(PlayerState player, WeaponState weapon, ShieldState shield, PlayerReticle playerReticle, EnemySpawner enemySpawner)
    {
        _player = player;
        _weapon = weapon;
        _shield = shield;
        _playerReticle = playerReticle;
        _enemySpawner = enemySpawner;
        _upgradeStates = new Dictionary<UpgradeId, UpgradeItemState>();
    }

    /// <summary>
    /// Initializes upgrade states from configuration data.
    /// Called at game start to populate available upgrades.
    /// </summary>
    public void Initialize(UpgradeItemConfig[] allUpgradeItemData)
    {
        foreach (UpgradeItemConfig data in allUpgradeItemData)
        {
            _upgradeStates[data.Id] = new UpgradeItemState(data);
        }
    }

    /// <summary>
    /// Gets the runtime state for a specific upgrade.
    /// </summary>
    public UpgradeItemState GetUpgradeItemState(UpgradeId id)
    {
        if (_upgradeStates.TryGetValue(id, out UpgradeItemState state))
        {
            return state;
        }

        Debug.LogError($"No upgrade state found for {id}");
        return null;
    }

    public Dictionary<UpgradeId, UpgradeItemState> GetAllUpgradeStates()
    {
        return _upgradeStates;
    }

    /// <summary>
    /// Restores upgrade states from saved game data.
    /// Reapplies all purchased upgrades to player stats.
    /// </summary>
    public void RestoreUpgradeStates(Dictionary<UpgradeId, int> savedUpgrades)
    {
        foreach (var kvp in savedUpgrades)
        {
            UpgradeItemState state = GetUpgradeItemState(kvp.Key);
            if (state != null)
            {
                state.UpdateStateFromLevel(kvp.Value);
                ApplyUpgrade(kvp.Key, state);
            }
        }
    }

    /// <summary>
    /// Attempts to purchase an upgrade level if player can afford it.
    /// </summary>
    /// <param name="canAffordCallback">Function to check if player has enough currency</param>
    /// <param name="deductCurrencyCallback">Function to deduct currency on successful purchase</param>
    /// <returns>True if purchase succeeded, false otherwise</returns>
    public bool TryPurchaseUpgrade(UpgradeId id, Func<int, CurrencyType, bool> canAffordCallback, Action<int, CurrencyType> deductCurrencyCallback)
    {
        UpgradeItemState state = GetUpgradeItemState(id);

        if (state != null || !state.IsUnlocked)
        {
            if (canAffordCallback(state.NextUpgradeCost, state.GetData().UpgradeCurrencyType))
            {
                deductCurrencyCallback(state.NextUpgradeCost, state.GetData().UpgradeCurrencyType);
                state.Upgrade();
                ApplyUpgrade(id, state);
                return true;
            }
            else
            {
                Debug.Log($"Not enough currency to upgrade {id}");
                return false;
            }
        }

        Debug.LogError($"No upgrade state found for {id}");
        return false;
    }

    /// <summary>
    /// Attempts to unlock an upgrade if player can afford the unlock cost.
    /// </summary>
    public bool TryUnlockUpgrade(UpgradeId id, Func<int, CurrencyType, bool> canAffordCallback, Action<int, CurrencyType> deductCurrencyCallback)
    {
        UpgradeItemState state = GetUpgradeItemState(id);

        if (state != null && !state.IsUnlocked)
        {
            if (canAffordCallback(state.GetData().UnlockCost, state.GetData().UnlockCurrencyType))
            {
                deductCurrencyCallback(state.GetData().UnlockCost, state.GetData().UnlockCurrencyType);
                state.IsUnlocked = true;
                return true;
            }
            else
            {
                Debug.Log($"Not enough currency to unlock {id}");
                return false;
            }
        }

        Debug.LogError($"No upgrade state found for {id} or already unlocked");
        return false;
    }

    /// <summary>
    /// Applies upgrade effects to the appropriate game systems.
    /// Calculates delta from current value to avoid double-applying on load.
    /// </summary>
    private void ApplyUpgrade(UpgradeId upgradeId, UpgradeItemState state)
    {
        switch (upgradeId)
        {
            case UpgradeId.PlayerHealth:
                float healthIncrease = state.CurrentValue - _player.MaxHealth;
                if (healthIncrease > 0)
                {
                    _player.IncreaseMaxHealth(healthIncrease);
                }
                break;
            case UpgradeId.SquarePulseDamagePerShot:
                _weapon.UpdateAttackDamage(state.CurrentValue - _weapon.AttackDamage);
                break;
            case UpgradeId.SquarePulseAttackSpeed:
                _weapon.UpdateAttackSpeed(state.CurrentValue - _weapon.AttackSpeed);
                break;
            case UpgradeId.SquarePulseCritChance:
                _weapon.UpdateCritChance(state.CurrentValue - _weapon.CritChance);
                break;
            case UpgradeId.SquarePulseCritDamageMultiplier:
                _weapon.UpdateCritDamageMultiplier(state.CurrentValue - _weapon.CritDamageMultiplier);
                break;
            case UpgradeId.SquarePulseReticleSize:
                _weapon.UpdateReticleSize((int)state.CurrentValue - _weapon.ReticleSize);
                _playerReticle.SetReticleSize(state.CurrentValue);
                break;
            case UpgradeId.StandardShieldStrength:
                _shield.UpdateMaxShieldStrength(state.CurrentValue - _shield.MaxShieldStrength);
                break;
            case UpgradeId.StandardShieldRegenRate:
                _shield.UpdateRegenRate(state.CurrentValue - _shield.RegenRate);
                break;
            case UpgradeId.StandardShieldHealthBonus:
                _shield.UpdateHealthBonus((int)state.CurrentValue);
                _player.SetBonusHealthFromShield(_shield.HealthBonus);
                break;
            case UpgradeId.StandardShieldAbsorbChance:
                _shield.UpdateAbsorbChance(state.CurrentValue - _shield.AbsorbChance);
                break;
        }
    }
}