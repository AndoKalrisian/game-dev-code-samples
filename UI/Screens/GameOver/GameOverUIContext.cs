using UnityEngine;

public class GameOverUIContext
{
    public int SoftCurrencyCollected { get; private set; }
    public int HardCurrencyCollected { get; private set; }
    public int EnemiesKilled { get; private set; }
    public float SessionTime { get; private set; }

    public GameOverUIContext(GameSessionState sessionData)
    {
        SoftCurrencyCollected = sessionData.SoftCurrencyCollected;
        HardCurrencyCollected = sessionData.HardCurrencyCollected;
        EnemiesKilled = sessionData.EnemiesKilled;
        SessionTime = sessionData.SessionTime;
    }

    public string GetFormattedSessionTime()
    {
        int minutes = Mathf.FloorToInt(SessionTime / 60f);
        int seconds = Mathf.FloorToInt(SessionTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
