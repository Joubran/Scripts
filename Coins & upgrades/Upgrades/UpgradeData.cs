using System;
using UnityEngine;

public enum UpgradeType {
    Energy,
    Exp
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Clicker/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("References")]
    public GameObject upgradePrefab;
    public Sprite sprite;
    public Collectable collectable;

    [Header("Tweaks")]
    public Vector3 position;
    public string upgradeDisplayName;
    public UpgradeType upgradeType;     // NEW: specify if this is an Energy or Exp upgrade

    [Header("Ingame")]
    public CoinAmount upgradeCost = new CoinAmount(0d, 0);
    public CoinAmount generationAmount;
    public bool isObtained = false;
    public int upgradeLevel = 1;
    public float costGrowthFactor = 1.12f;
    public float generationGrowthMultiplier = 1.06f;

    public event Action<CoinAmount> CostChanged;

    public void Upgrade()
    {
        upgradeLevel++;
        generationAmount = CalculateNextGenerationAmount();
    }

    public void SetUpgradeCost(CoinAmount newCost)
    {
        upgradeCost = newCost;
        CostChanged?.Invoke(upgradeCost);
    }

    public CoinAmount CalculateNextGenerationAmount()
    {
        double newMantissa = generationAmount.mantissa * generationGrowthMultiplier;
        int newExponent = generationAmount.exponent;

        while (newMantissa >= 1000d)
        {
            newMantissa /= 1000d;
            newExponent++;
        }

        return new CoinAmount(newMantissa, newExponent);
    } 
}
