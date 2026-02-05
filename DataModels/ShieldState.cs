using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages shield state and behavior for the player.
/// Implements pseudo-random distribution (PRD) for damage absorption to create more consistent gameplay.
/// Does not contain visual or input logic - purely data management.
/// </summary>
public class ShieldState
{
    #region Properties and Events
    
    /// <summary>
    /// Base maximum shield strength before any modifiers are applied.
    /// This is the configured value that can be upgraded over time.
    /// </summary>
    public float MaxShieldStrength { get; private set; }

    /// <summary>
    /// Effective maximum shield strength after applying all active stat modifiers.
    /// This is the actual max value used for gameplay calculations.
    /// </summary>
    private float _effectiveMaxShieldStrength;

    /// <summary>
    /// Public accessor for effective maximum shield strength.
    /// </summary>
    public float EffectiveMaxShieldStrength => _effectiveMaxShieldStrength;
    
    /// <summary>
    /// Current shield strength value. When this reaches 0, the shield is depleted.
    /// Regenerates over time based on RegenRate.
    /// </summary>
    public float CurrentShieldStrength { get; private set; }
    
    /// <summary>
    /// Health bonus provided by the shield when active.
    /// Applied to player's max health while shield is equipped.
    /// </summary>
    public int HealthBonus { get; private set; }
    
    /// <summary>
    /// Base chance for the shield to completely absorb incoming damage (0-1 range).
    /// Used with pseudo-random distribution for more consistent gameplay feel.
    /// </summary>
    public float AbsorbChance { get; private set; }
    
    /// <summary>
    /// Shield regeneration rate per second.
    /// Automatically restores shield strength when not taking damage.
    /// </summary>
    public float RegenRate { get; private set; }
    
    /// <summary>
    /// Drop rate for shield booster pickups (0-1 range).
    /// Determines how frequently shield restoration items appear.
    /// </summary>
    public float BoosterDropRate { get; private set; }

    /// <summary>
    /// Current accumulated absorb chance for pseudo-random distribution (PRD).
    /// Increases after each non-absorb event, resets to 0 after successful absorb.
    /// Ensures more consistent absorb timing compared to pure random chance.
    /// </summary>
    private float _absorbAccumulator;
    
    /// <summary>
    /// Increment value added to absorb accumulator per non-absorb event.
    /// Calculated based on desired average absorb rate using simplified PRD formula.
    /// </summary>
    private float _absorbIncrement;

    /// <summary>
    /// Fired when shield strength changes. Parameters: (shield percentage 0-1, current shield strength)
    /// Used by UI to update shield bar displays.
    /// </summary>
    public event System.Action<float, float> OnShieldStrengthChanged;
    
    /// <summary>
    /// Fired when shield is depleted (reaches 0 strength).
    /// Triggers visual/audio feedback and potentially removes health bonus.
    /// </summary>
    public event System.Action OnShieldDepleted;
    
    /// <summary>
    /// Fired when shield is restored to full strength from depleted state.
    /// Triggers restoration effects and re-applies health bonus.
    /// </summary>
    public event System.Action OnShieldRestored;

    /// <summary>
    /// Fired when damage is absorbed by the shield. Parameters: (damage amount absorbed)
    /// Used for visual feedback and statistics tracking.
    /// </summary>
    public event System.Action<float> OnDamageAbsorbed;

    #endregion
    
    #region Initialization

    /// <summary>
    /// Sets up initial shield values from configuration.
    /// Called once at game start or when loading a new game.
    /// Initializes PRD system and sets all base values.
    /// </summary>
    /// <param name="config">Shield configuration containing initial values</param>
    public void Initialize(ShieldConfig config)
    {
        MaxShieldStrength = config.StandardShieldStrength;
        CurrentShieldStrength = MaxShieldStrength;
        HealthBonus = config.StandardShieldHealthBonus;
        AbsorbChance = config.StandardShieldAbsorbChance * 0.01f; // Convert percentage to decimal
        RegenRate = config.StandardShieldRegenRate;
        BoosterDropRate = config.StandardShieldBoosterDropRate;
        
        // Initialize PRD system for damage absorption
        UpdateAbsorbIncrement();
        _absorbAccumulator = 0f;
    }

    #endregion

    #region Save/Load Methods

    /// <summary>
    /// Restores shield state from saved session data.
    /// Used when loading a saved game to restore previous shield strength.
    /// </summary>
    /// <param name="savedData">Saved session data containing shield state</param>
    public void RestoreShieldState(SaveLoadSessionData savedData)
    {
        CurrentShieldStrength = savedData.CurrentShield;
    }

    /// <summary>
    /// Sets the current shield strength directly without triggering events.
    /// Used for loading saved games or debug/testing purposes.
    /// </summary>
    /// <param name="strength">Shield strength value to set</param>
    public void SetCurrentShieldStrength(float strength)
    {
        CurrentShieldStrength = strength;
    }

    #endregion

    #region Damage and Regeneration

    /// <summary>
    /// Processes incoming damage using pseudo-random distribution for absorption.
    /// First checks if damage is completely absorbed, then drains shield strength.
    /// Uses PRD accumulator to create more predictable absorption timing.
    /// </summary>
    /// <param name="amount">Amount of damage to process</param>
    /// <returns>Amount of damage that was not absorbed and should be applied to health</returns>
    public float TakeDamage(float amount)
    {
        // Check for complete damage absorption using PRD accumulator
        bool isAbsorbed = Random.value < _absorbAccumulator;
        if (isAbsorbed)
        {
            Debug.Log($"ShieldState: Absorbed {amount} damage.");
            // Damage completely absorbed - reset accumulator for next cycle
            _absorbAccumulator = 0f;
            return 0f; // No damage passes through
        }

        // Damage not absorbed - increase accumulator for next time
        // Caps at 1.0 to prevent overflow
        _absorbAccumulator = Mathf.Min(1f, _absorbAccumulator + _absorbIncrement);
        
        float actualDamageAbsorbed = 0;
        float damageRemaining = 0;

        // Drain shield strength if any remains
        if (CurrentShieldStrength > 0)
        {
            // Absorb as much damage as possible with remaining shield
            actualDamageAbsorbed = Mathf.Min(amount, CurrentShieldStrength);
            CurrentShieldStrength = Mathf.Max(0, CurrentShieldStrength - amount);
            damageRemaining = amount - actualDamageAbsorbed;
            
            // Notify listeners of shield strength change
            OnShieldStrengthChanged?.Invoke(GetShieldPercentage(), CurrentShieldStrength);
            
            // Check if shield was fully depleted
            if (CurrentShieldStrength <= 0)
            {
                OnShieldDepleted?.Invoke();
            }
        }
        else
        {
            // Shield already depleted - all damage passes through
            damageRemaining = amount;
        }
        
        return damageRemaining;
    }

    /// <summary>
    /// Regenerates shield strength over time based on RegenRate.
    /// Called each frame/update tick when shield is below max.
    /// Triggers restoration event when shield returns from 0.
    /// </summary>
    public void RegenerateShield()
    {
        if (CurrentShieldStrength < EffectiveMaxShieldStrength)
        {
            bool wasZero = CurrentShieldStrength == 0;
            
            // Regenerate shield, capped at effective max
            CurrentShieldStrength = Mathf.Min(EffectiveMaxShieldStrength, CurrentShieldStrength + RegenRate);
            OnShieldStrengthChanged?.Invoke(GetShieldPercentage(), CurrentShieldStrength);
            
            // Check if shield was restored from depleted state
            if (wasZero && CurrentShieldStrength > 0)
            {
                OnShieldRestored?.Invoke();
            }
        }
    }

    /// <summary>
    /// Instantly restores shield strength by a specific amount.
    /// Used for shield booster pickups or special abilities.
    /// Capped at effective maximum shield strength.
    /// </summary>
    /// <param name="amount">Amount of shield strength to restore</param>
    public void RestoreShield(float amount)
    {
        CurrentShieldStrength = Mathf.Min(EffectiveMaxShieldStrength, CurrentShieldStrength + amount);
        OnShieldStrengthChanged?.Invoke(GetShieldPercentage(), CurrentShieldStrength);
    }

    /// <summary>
    /// Resets shield to full strength and recalculates effective max.
    /// Used when restarting a level or respawning player.
    /// </summary>
    public void Reset()
    {
        UpdateEffectiveMaxShieldStrength();
        CurrentShieldStrength = EffectiveMaxShieldStrength;
        OnShieldStrengthChanged?.Invoke(GetShieldPercentage(), CurrentShieldStrength);
    }

    #endregion

    #region Stat Updates (Upgrades)

    /// <summary>
    /// Updates the base maximum shield strength.
    /// Used for permanent shield capacity upgrades.
    /// Does not affect current shield strength - only maximum capacity.
    /// </summary>
    /// <param name="delta">Change in maximum shield strength value</param>
    public void UpdateMaxShieldStrength(float delta)
    {
        MaxShieldStrength += delta;
    }

    /// <summary>
    /// Recalculates effective maximum shield strength from base value and all modifiers.
    /// Applies additive modifiers first, then multiplicative modifiers.
    /// </summary>
    private void UpdateEffectiveMaxShieldStrength()
    {
        float value = MaxShieldStrength;
        
        // Apply all stat modifiers if any exist
        if (_maxShieldStrengthModifiers != null)
        {
            foreach (StatModifier mod in _maxShieldStrengthModifiers)
            {
                value += mod.AdditiveBonus;              // Apply flat bonuses first
                value *= (1f + mod.MultiplicativeBonus); // Then apply percentage bonuses
            }
        }

        _effectiveMaxShieldStrength = value;
    }

    /// <summary>
    /// Updates the health bonus provided by the shield.
    /// This bonus is typically added to player's max health while shield is active.
    /// </summary>
    /// <param name="value">New health bonus value</param>
    public void UpdateHealthBonus(int value)
    {
        HealthBonus = value;
        Debug.Log($"ShieldState: Updated HealthBonus to {HealthBonus}");
    }

    /// <summary>
    /// Calculates the absorb increment for pseudo-random distribution.
    /// Uses simplified approximation: increment = baseAbsorb * 0.5
    /// This makes the actual absorb rate slightly lower early but averages out correctly over time.
    /// PRD provides more consistent gameplay feel than pure random chance.
    /// </summary>
    private void UpdateAbsorbIncrement()
    {
        _absorbIncrement = AbsorbChance * 0.5f;
    }

    /// <summary>
    /// Updates the shield's damage absorption chance.
    /// Recalculates PRD increment to maintain proper distribution.
    /// </summary>
    /// <param name="delta">Change in absorption chance percentage (e.g., 5 for +5%)</param>
    public void UpdateAbsorbChance(float delta)
    {
        AbsorbChance = Mathf.Clamp01(AbsorbChance + delta * 0.01f); // Convert percentage and clamp
        UpdateAbsorbIncrement(); // Recalculate PRD increment
    }

    /// <summary>
    /// Updates the shield regeneration rate.
    /// Higher values restore shield strength faster.
    /// </summary>
    /// <param name="delta">Change in regeneration rate per second</param>
    public void UpdateRegenRate(float delta)
    {
        RegenRate += delta;
    }

    /// <summary>
    /// Updates the shield booster drop rate.
    /// Affects how frequently shield restoration pickups appear.
    /// </summary>
    /// <param name="delta">Change in drop rate (0-1 range)</param>
    public void UpdateBoosterDropRate(float delta)
    {
        BoosterDropRate = Mathf.Clamp01(BoosterDropRate + delta);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Calculates current shield strength as a percentage (0-1) for UI display.
    /// Used by health bars and UI elements to show shield status.
    /// </summary>
    /// <returns>Shield percentage between 0 and 1</returns>
    public float GetShieldPercentage()
    {
        return EffectiveMaxShieldStrength > 0 ? (float)CurrentShieldStrength / EffectiveMaxShieldStrength : 0f;
    }

    #endregion

    #region Modifier System

    /// <summary>
    /// List of temporary stat modifiers affecting maximum shield strength.
    /// Used for power-ups, game modifiers, or temporary effects.
    /// </summary>
    private List<StatModifier> _maxShieldStrengthModifiers;

    /// <summary>
    /// Adds a temporary stat modifier to maximum shield strength.
    /// Recalculates effective max and optionally restores shield to new maximum.
    /// </summary>
    /// <param name="modifier">Stat modifier to apply (additive and/or multiplicative)</param>
    /// <param name="restoreToFull">If true, sets current shield to new maximum value</param>
    public void AddMaxShieldStrengthModifier(StatModifier modifier, bool restoreToFull = false)
    {
        // Initialize modifier list on first use
        if (_maxShieldStrengthModifiers == null)
        {
            _maxShieldStrengthModifiers = new List<StatModifier>();
        }

        _maxShieldStrengthModifiers.Add(modifier);
        UpdateEffectiveMaxShieldStrength();

        // Optionally restore shield to new maximum
        if (restoreToFull)
        {
            CurrentShieldStrength = EffectiveMaxShieldStrength;
            OnShieldStrengthChanged?.Invoke(GetShieldPercentage(), CurrentShieldStrength);
        }
    }

    /// <summary>
    /// Removes all temporary stat modifiers.
    /// Used when session ends or modifiers expire.
    /// Recalculates effective max back to base value.
    /// </summary>
    public void ClearAllModifiers()
    {
        if (_maxShieldStrengthModifiers != null)
        {
            _maxShieldStrengthModifiers.Clear();
            UpdateEffectiveMaxShieldStrength();
        }
    }

    #endregion
}