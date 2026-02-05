using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks gameplay statistics and progression for the current game session.
/// </summary>
public class GameSessionState
{
    /// <summary>
    /// Current player level in this session.
    /// </summary>
    public int CurrentLevel { get; set; }

    /// <summary>
    /// Time elapsed in the current level in seconds.
    /// </summary>
    public float LevelTimeElapsedInSeconds { get; set; }
    
    /// <summary>
    /// Total number of enemies killed in this session.
    /// </summary>
    public int EnemiesKilled { get; private set; }
    
    /// <summary>
    /// Total time elapsed in seconds for this session.
    /// </summary>
    public float SessionTime { get; private set; }
    
    /// <summary>
    /// Highest combo achieved during this session.
    /// </summary>
    public int HighestCombo { get; private set; }
    
    /// <summary>
    /// Current active combo count.
    /// </summary>
    public int CurrentCombo { get; private set; }
    
    /// <summary>
    /// Total soft currency collected (not added to permanent inventory).
    /// </summary>
    public int SoftCurrencyCollected { get; private set; }
    
    /// <summary>
    /// Total hard currency collected (not added to permanent inventory).
    /// </summary>
    public int HardCurrencyCollected { get; private set; }

    /// <summary>
    /// Total game modifier currency collected (not added to permanent inventory).
    /// </summary>
    public int GameModifierCurrencyCollected { get; private set; }

    public List<GameModifierId> AllSelectedGameModifierIds = new List<GameModifierId>();
    
    /// <summary>
    /// Fired when an enemy is killed. Parameter: total enemies killed
    /// </summary>
    public event System.Action<int> OnEnemyKilled;
    
    /// <summary>
    /// Fired when combo count changes. Parameter: current combo
    /// </summary>
    public event System.Action<int> OnComboChanged;
    
    /// <summary>
    /// Fired when a new highest combo is achieved. Parameter: new highest combo
    /// </summary>
    public event System.Action<int> OnHighestComboChanged;
    
    /// <summary>
    /// Fired when soft currency is collected. Parameter: total soft currency
    /// </summary>
    public event System.Action<int> OnSoftCurrencyCollected;
    
    /// <summary>
    /// Fired when hard currency is collected. Parameter: total hard currency
    /// </summary>
    public event System.Action<int> OnHardCurrencyCollected;
    
    /// <summary>
    /// Fired when game modifier currency is collected. Parameter: total game modifier currency
    /// </summary>
    public event System.Action<int> OnGameModifierCurrencyCollected;
    
    /// <summary>
    /// Resets all session statistics to zero.
    /// </summary>
    public void Initialize()
    {
        CurrentLevel = 1;
        LevelTimeElapsedInSeconds = 0f;
        EnemiesKilled = 0;
        SessionTime = 0f;
        HighestCombo = 0;
        CurrentCombo = 0;
        SoftCurrencyCollected = 0;
        HardCurrencyCollected = 0;
        GameModifierCurrencyCollected = 0;
    }

    public void RestoreSessionState(SaveLoadSessionData data)
    {
        CurrentLevel = data.LevelNumber;
        LevelTimeElapsedInSeconds = data.LevelTimeElapsedInSeconds;
        EnemiesKilled = data.EnemiesKilled;
        SessionTime = data.SessionTime;
        HighestCombo = data.HighestCombo;
        CurrentCombo = data.CurrentCombo;
        SoftCurrencyCollected = data.SoftCurrencyCollected;
        HardCurrencyCollected = data.HardCurrencyCollected;
        GameModifierCurrencyCollected = data.GameModifierCurrencyCollected;
        AllSelectedGameModifierIds = data.AllSelectedGameModifierIds;
    }

    /// <summary>
    /// Updates the total session time. Called every frame by GameManager.
    /// </summary>
    /// <param name="deltaTime">Time since last frame</param>
    public void UpdateSessionTime(float deltaTime)
    {
        SessionTime += deltaTime;
    }

    public void UpdateCurrentLevel(int level)
    {
        CurrentLevel = level;
    }

    /// <summary>
    /// Records an enemy kill and increments combo counter.
    /// Updates highest combo if current combo exceeds it.
    /// </summary>
    public void RecordEnemyKilled()
    {
        EnemiesKilled++;
        CurrentCombo++;
        
        if (CurrentCombo > HighestCombo)
        {
            HighestCombo = CurrentCombo;
            OnHighestComboChanged?.Invoke(HighestCombo);
        }
        
        OnEnemyKilled?.Invoke(EnemiesKilled);
        OnComboChanged?.Invoke(CurrentCombo);
        
    }

    /// <summary>
    /// Resets the current combo counter to zero. Called when player misses or takes damage.
    /// </summary>
    public void ResetCombo()
    {
        CurrentCombo = 0;
        OnComboChanged?.Invoke(CurrentCombo);
    }

    /// <summary>
    /// Resets all session data. Alias for Initialize().
    /// </summary>
    public void Reset()
    {
        Initialize();
    }

    /// <summary>
    /// Records soft currency collected during gameplay.
    /// </summary>
    /// <param name="amount">Amount of soft currency collected</param>
    public void RecordSoftCurrencyCollected(int amount)
    {
        SoftCurrencyCollected += amount;
        OnSoftCurrencyCollected?.Invoke(SoftCurrencyCollected);
    }
    
    /// <summary>
    /// Records hard currency collected during gameplay.
    /// </summary>
    /// <param name="amount">Amount of hard currency collected</param>
    public void RecordHardCurrencyCollected(int amount)
    {
        HardCurrencyCollected += amount;
        OnHardCurrencyCollected?.Invoke(HardCurrencyCollected);
    }

    /// <summary>
    /// Records game modifier currency collected during gameplay.
    /// </summary>
    /// <param name="amount">Amount of game modifier currency collected</param>
    public void RecordGameModifierCurrencyCollected(int amount)
    {
        GameModifierCurrencyCollected += amount;
        OnGameModifierCurrencyCollected?.Invoke(GameModifierCurrencyCollected);
    }

    public void UpdateGameModifierIds(List<GameModifierId> modifierIds)
    {
        AllSelectedGameModifierIds = modifierIds;
    }
    
    /// <summary>
    /// Resets all session data. Alias for Initialize().
    /// </summary>
    public void ResetSession()
    {
        Initialize();
    }
}