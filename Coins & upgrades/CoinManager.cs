using System;
using TMPro;
using UnityEngine;

/// Represents a coin amount as a mantissa (<1000) and an exponent index for powers of 1000.

[Serializable]
public struct CoinAmount {
    public double mantissa;
    public int exponent;

    public CoinAmount(double mantissa, int exponent) {
        this.mantissa = mantissa;
        this.exponent = exponent;
        Normalize();
    }

    private void Normalize() {
        if (mantissa < 0) {
            mantissa = 0;
            exponent = 0;
            return;
        }
        while (mantissa >= 1000d) {
            mantissa /= 1000d;
            exponent++;
        }
        while (mantissa > 0 && mantissa < 1d && exponent > 0) {
            mantissa *= 1000d;
            exponent--;
        }
    }

    public static CoinAmount Add(CoinAmount a, CoinAmount b) {
        if (a.exponent > b.exponent) {
            b.mantissa /= Math.Pow(1000d, a.exponent - b.exponent);
            b.exponent = a.exponent;
        } else if (b.exponent > a.exponent) {
            a.mantissa /= Math.Pow(1000d, b.exponent - a.exponent);
            a.exponent = b.exponent;
        }
        var result = new CoinAmount(a.mantissa + b.mantissa, a.exponent);
        result.Normalize();
        return result;
    }

    public static CoinAmount Subtract(CoinAmount a, CoinAmount b) {
        if (a.exponent > b.exponent) {
            b.mantissa /= Math.Pow(1000d, a.exponent - b.exponent);
            b.exponent = a.exponent;
        } else if (b.exponent > a.exponent) {
            a.mantissa /= Math.Pow(1000d, b.exponent - a.exponent);
            a.exponent = b.exponent;
        }
        double diff = a.mantissa - b.mantissa;
        if (diff <= 0d) {
            return new CoinAmount(0d, 0);
        }
        var result = new CoinAmount(diff, a.exponent);
        result.Normalize();
        return result;
    }

    public string Format() {
            string[] suffixes = {
            "", "a", "b", "c", "d", "e", "f", "g", "h", "i",
            "j", "k", "l", "m", "n", "o", "p", "q", "r", "s",
            "t", "u", "v", "w", "x", "y", "z"
            };
        int idx = Mathf.Clamp(exponent, 0, suffixes.Length - 1);
        return mantissa.ToString("F2") + suffixes[idx];
    }
}

/// Manages the player's gold coins using CoinAmount representation.

public class CoinManager : MonoBehaviour {
    public static CoinManager Instance { get; private set; }

    [Header("References")]
    public TMP_Text coinsCounterText;
    public TMP_Text energyCounterText;

    [SerializeField]
    private CoinAmount totalCoins = new CoinAmount(0d, 0);
    private CoinAmount totalEnergy = new CoinAmount(0d, 0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetFormattedCoins(CoinAmount coinAmount) {
        return coinAmount.Format();
    }

    // coin methods
    public void AddCoins(CoinAmount amount)
    {
        totalCoins = CoinAmount.Add(totalCoins, amount);
        UpdateUI();
    }

    public bool UseCoins(CoinAmount amount) {
        var newTotal = CoinAmount.Subtract(totalCoins, amount);
        bool success = !(newTotal.mantissa == 0d && newTotal.exponent == 0 && (amount.mantissa > totalCoins.mantissa || amount.exponent > totalCoins.exponent));
        if (success) {
            totalCoins = newTotal;
            UpdateUI();
        }
        return success;
    }

    public bool HasCoins(CoinAmount amount) {
        var newTotal = CoinAmount.Subtract(totalCoins, amount);
        bool canAfford = !(newTotal.mantissa == 0d && newTotal.exponent == 0 && 
                           (amount.mantissa > totalCoins.mantissa || amount.exponent > totalCoins.exponent));
        return canAfford;
    }

    // energy methods

    public void AddEnergy(CoinAmount amount)
    {
        totalEnergy = CoinAmount.Add(totalEnergy, amount);
        UpdateUI();
    }

    public bool UseEnergy(CoinAmount amount) {
        var newTotal = CoinAmount.Subtract(totalEnergy, amount);
        bool success = !(newTotal.mantissa == 0d && newTotal.exponent == 0 && (amount.mantissa > totalEnergy.mantissa || amount.exponent > totalEnergy.exponent));
        if (success) {
            totalEnergy = newTotal;
            UpdateUI();
        }
        return success;
    }
    private void UpdateUI()
    {
        coinsCounterText.text = GetFormattedCoins(totalCoins);
        energyCounterText.text = GetFormattedCoins(totalEnergy);
    }

    [ContextMenu("Test CoinManager Advanced")]
    private void Test() {
        totalCoins = new CoinAmount(999, 0);
        Debug.Log(GetFormattedCoins(totalCoins));

        AddCoins(new CoinAmount(1, 3));
        Debug.Log(GetFormattedCoins(totalCoins));

        bool spent = UseCoins(new CoinAmount(0.5, 0));
        Debug.Log($"Spent 0.5C? {spent}. Remaining: " + GetFormattedCoins(totalCoins));
    }
}