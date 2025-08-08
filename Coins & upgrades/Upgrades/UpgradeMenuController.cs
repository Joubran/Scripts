using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenuController : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    public GameObject listItemPrefab;        // should have UpgradeListItem component

    public GameObject energySelectedBG;
    public GameObject expSelectedBG;

    [Header("UI Containers")]
    public RectTransform energyContent;      // Content panel under Energy tab
    public RectTransform expContent;         // Content panel under Exp tab

    [Header("Tab Buttons")]
    public Button energyTabButton;
    public Button expTabButton;

    [Header("Bottom Upgrade Button")]
    public UpgradeButton bottomUpgradeButton; // reference to your existing cheapest-upgrade button

    private void Awake()
    {
        energyTabButton.onClick.AddListener(() => ShowTab(UpgradeType.Energy));
        expTabButton.onClick.AddListener(() => ShowTab(UpgradeType.Exp));
    }

    private void OnEnable()
    {
        PopulateLists();
        ShowTab(UpgradeType.Energy);
    }

    private void PopulateLists()
    {
        ClearContainer(energyContent);
        ClearContainer(expContent);

        foreach (UpgradeData data in upgradeManager.upgrades)
        {
            if (!data.isObtained)
                continue;

            // choose parent based on type
            RectTransform parent = data.upgradeType == UpgradeType.Energy ? energyContent : expContent;
            GameObject go = Instantiate(listItemPrefab, parent);
            UpgradeListItem item = go.GetComponent<UpgradeListItem>();
            item.Initialize(data, this);
        }
    }

    private void ClearContainer(RectTransform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    public void TryBuySpecific(UpgradeData data, UpgradeListItem item)
    {
        // use same cost-calculation as UpgradeButton
        if (CoinManager.Instance.UseCoins(data.upgradeCost))
        {
            data.Upgrade();
            data.upgradeCost = CalculateNextCost(data);

            // update generation on the spawned object
            var obj = upgradeManager.upgradeObjects.Find(o => o.data == data);
            obj?.UpdateGenerationAmount();

            // refresh list item UI
            item.UpdateUI();

            // refresh cheapest button at bottom
            bottomUpgradeButton.PickCheapestUpgrade();
            bottomUpgradeButton.UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade.");
        }
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

    private void ShowTab(UpgradeType type)
    {
        energyContent.parent.parent.gameObject.SetActive(type == UpgradeType.Energy);
        energySelectedBG.SetActive(type == UpgradeType.Energy);

        expContent.parent.parent.gameObject.SetActive(type == UpgradeType.Exp);
        expSelectedBG.SetActive(type == UpgradeType.Exp);
    }
}
