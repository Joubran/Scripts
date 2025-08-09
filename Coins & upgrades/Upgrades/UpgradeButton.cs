using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    public UpgradeData upgradeData;
    public UpgradeObject upgradeObject;
    public TMP_Text levelText;
    public TMP_Text costText;
    public TMP_Text upgradeNameText;
    public TMP_Text upgradeDescriptionText;
    public Image upgradeIconImage;
    public Button upgradeButton;


    private void Start()
    {
        if (upgradeData == null)
        {
            upgradeData = upgradeManager.upgrades[0];
            PickCheapestUpgrade();
        }
        UpdateUI();
    }

    public void BuyUpgrade()
    {
        CoinAmount cost = upgradeData.upgradeCost;

        if (CoinManager.Instance.UseCoins(cost))
        {
            upgradeData.Upgrade();
            upgradeData.SetUpgradeCost(CalculateNextCost(upgradeData));
            upgradeObject.UpdateGenerationAmount();
            PickCheapestUpgrade();
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade.");
        }
    }


    public void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Lvl " + upgradeData.upgradeLevel.ToString();

        if (costText != null)
            costText.text = upgradeData.upgradeCost.Format();

        if (upgradeIconImage != null)
            upgradeIconImage.sprite = upgradeData.sprite;

        if (upgradeNameText != null)
            upgradeNameText.text = upgradeData.upgradeDisplayName;

        CoinAmount nextGen = upgradeData.CalculateNextGenerationAmount();

        if (upgradeDescriptionText != null)
            upgradeDescriptionText.text = "+" + $"{CoinAmount.Subtract(nextGen, upgradeData.generationAmount).Format()}" + "Energy!";

    }

    private CoinAmount CalculateNextCost(UpgradeData data)
    {
        double newMantissa = data.upgradeCost.mantissa * data.costGrowthFactor;
        int newExponent = data.upgradeCost.exponent;
        
        while (newMantissa >= 1000d)
        {
            newMantissa /= 1000d;
            newExponent++;
        }

        return new CoinAmount(newMantissa, newExponent);
    }
    public void PickCheapestUpgrade()
    {
        for (int i = 0; i < upgradeManager.upgradeObjects.Count; i++)
        {
            UpgradeData newData = upgradeManager.upgradeObjects[i].data;
            // if the cost we have isn't the lowest, find the lowest upgrade cost
            if (upgradeData.upgradeCost.exponent > newData.upgradeCost.exponent)
            {
                upgradeData = newData;
                upgradeObject = upgradeManager.upgradeObjects[i];
            }
            else if ((upgradeData.upgradeCost.exponent == newData.upgradeCost.exponent)
            && upgradeData.upgradeCost.mantissa >= newData.upgradeCost.mantissa)
            {
                upgradeData = newData;
                upgradeObject = upgradeManager.upgradeObjects[i];
            }
        }
    }
}
