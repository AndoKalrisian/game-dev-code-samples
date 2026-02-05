using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataModels/CurrencyConfig")]
public class CurrencyConfig : ScriptableObject
{
    public int SoftCurrency;
    public int HardCurrency;
}