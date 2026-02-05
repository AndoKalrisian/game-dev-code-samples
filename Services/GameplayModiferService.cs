using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

/// <summary>
/// Manages temporary gameplay modifiers (power-ups) during a session.
/// Handles random selection, prevents repetition, and applies stat modifiers.
/// </summary>
public class GameplayModifierService
{
    private WeaponState _weapon;
    private ShieldState _shield;
    private PlayerState _player;
    private EnemySpawner _enemySpawner;
    private GameSessionState _sessionState;
    
    /// <summary>
    /// All available modifiers indexed by ID for quick lookup.
    /// </summary>
    private Dictionary<GameModifierId, GameplayModifierItemConfig> _allModifiersDict = new Dictionary<GameModifierId, GameplayModifierItemConfig>();
    
    /// <summary>
    /// Reusable buffer for filtering available modifiers (reduces allocations).
    /// </summary>
    private List<GameplayModifierItemConfig> _availableModifiersBuffer = new List<GameplayModifierItemConfig>();
    
    /// <summary>
    /// Tracks recently selected modifiers to prevent immediate repetition.
    /// </summary>
    private Queue<GameModifierId> _recentSelections = new Queue<GameModifierId>();
    
    /// <summary>
    /// All modifiers selected during current session (for save/restore).
    /// </summary>
    private List<GameModifierId> _allSelectedGameModifierIds = new List<GameModifierId>();
    
    /// <summary>
    /// Number of recent selections to exclude from random pool.
    /// </summary>
    private const int MAX_RECENT_SELECTIONS = 2;
    
    [Inject]
    public GameplayModifierService(WeaponState weapon, ShieldState shield, PlayerState player, EnemySpawner enemySpawner, GameSessionState sessionState)
    {
        _weapon = weapon;
        _shield = shield;
        _player = player;
        _enemySpawner = enemySpawner;
        _sessionState = sessionState;
    }

    /// <summary>
    /// Initializes the modifier dictionary from configuration data.
    /// </summary>
    public void CreateModifierDict(GameplayModifierItemConfig[] allModifiers)
    {
        _allModifiersDict = allModifiers.ToDictionary(modifier => modifier.Id, modifier => modifier);
    }

    /// <summary>
    /// Selects a random gameplay modifier, excluding recently selected ones.
    /// Clears recent selection history if all modifiers have been recently selected.
    /// </summary>
    public GameplayModifierItemConfig SelectRandomModifier()
    {
        if (_allModifiersDict == null || _allModifiersDict.Count == 0)
        {
            Debug.LogWarning("GameplayModifierService: No modifiers available for selection");
            return null;
        }

        // Build list of modifiers not recently selected
        _availableModifiersBuffer.Clear();
        
        foreach (GameplayModifierItemConfig modifier in _allModifiersDict.Values)
        {
            if (!_recentSelections.Contains(modifier.Id))
            {
                _availableModifiersBuffer.Add(modifier);
            }
        }

        // If all modifiers recently selected, reset and allow all
        if (_availableModifiersBuffer.Count == 0)
        {
            _recentSelections.Clear();
            _availableModifiersBuffer.AddRange(_allModifiersDict.Values);
        }

        // Select random modifier from available pool
        int randomIndex = UnityEngine.Random.Range(0, _availableModifiersBuffer.Count);
        GameplayModifierItemConfig selected = _availableModifiersBuffer[randomIndex];

        // Track selection to prevent immediate repetition
        _recentSelections.Enqueue(selected.Id);
        if (_recentSelections.Count > MAX_RECENT_SELECTIONS)
        {
            _recentSelections.Dequeue();
        }

        return selected;
    }
    
    /// <summary>
    /// Applies a gameplay modifier to the appropriate game systems.
    /// Tracks selection for session persistence.
    /// </summary>
    public void ApplyModifier(GameplayModifierItemConfig config)
    {
        StatModifier modifier = new StatModifier(
            additive: config.AdditiveValue,
            multiplicative: config.MultiplicativeValue,
            gameModifierId: config.Id
        );
        
        switch(config.Id)
        {
            case GameModifierId.RestoreHealth:
                _player.RestoreHealth(_player.EffectiveMaxHealth);
                break;
            case GameModifierId.MaxHealthAdditive:
                _player.AddMaxHealthModifier(modifier, restoreToFull: true);
                break;
            case GameModifierId.AttackDamagePercent:
                _weapon.AddAttackDamageModifier(modifier);
                break;
            case GameModifierId.BossAttackDamagePercent:
                _weapon.AddBossAttackDamageModifier(modifier);
                break;
            case GameModifierId.RestoreShield:
                _shield.RestoreShield(_shield.EffectiveMaxShieldStrength);
                break;
            case GameModifierId.MaxShieldAdditive:
                _shield.AddMaxShieldStrengthModifier(modifier, restoreToFull: true);
                break;
            case GameModifierId.SoftCurrencyDropBonusChance:
                _enemySpawner.AddSoftCurrencyBonusDropChanceModifier(modifier);
                _enemySpawner.AddSoftCurrencyBonusDropQuantityModifier(new StatModifier(
                    additive: 1f,
                    gameModifierId: config.Id));
                break;
            case GameModifierId.HardCurrencyEnemySpawnChance:
                _enemySpawner.AddHardCurrencyEnemySpawnRateModifier(modifier);
                break;
            default:
                Debug.LogWarning($"Unhandled Gameplay Modifier ID: {config.Id}");
                break;
        }

        // Track for session save/restore
        _allSelectedGameModifierIds.Add(config.Id);
        _sessionState.UpdateGameModifierIds(_allSelectedGameModifierIds);
    }

    public List<GameModifierId> GetAllSelectedModifierIds()
    {
        return _allSelectedGameModifierIds;
    }

    /// <summary>
    /// Restores previously selected modifiers from saved session data.
    /// Reapplies all modifiers in original selection order.
    /// </summary>
    public void RestoreSavedModifiersFromIds(List<GameModifierId> savedIds)
    {
        foreach (GameModifierId id in savedIds)
        {
            if (_allModifiersDict.TryGetValue(id, out GameplayModifierItemConfig config))
            {
                Debug.Log($"GameplayModifierService: Restoring saved modifier: {config.Id}");
                ApplyModifier(config);
            }
        }
    }   
    
    /// <summary>
    /// Clears all active modifiers from all game systems.
    /// Used when session ends or player dies.
    /// </summary>
    public void ClearAllModifiers()
    {
        _weapon.ClearAllModifiers();
        _shield.ClearAllModifiers();
        _player.ClearAllModifiers();
        _enemySpawner.ClearAllModifiers();
    }
}