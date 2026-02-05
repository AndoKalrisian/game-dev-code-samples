using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataModels/ShieldConfig")]
public class ShieldConfig : ScriptableObject
{
    public float StandardShieldStrength = 10;
    public int StandardShieldHealthBonus = 50;
    public float StandardShieldAbsorbChance = 0.1f;
    public float StandardShieldRegenRate = 0.1f;
    public float StandardShieldBoosterDropRate = 0.01f;
}