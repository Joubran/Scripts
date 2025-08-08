using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeListItem : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText, levelText, costText, descText;
    public Button buyButton;

    private UpgradeData data;
    private UpgradeMenuController menu;

    public void Initialize(UpgradeData data, UpgradeMenuController menu)
    {
        this.data = data;
        this.menu = menu;
        buyButton.onClick.AddListener(OnBuyClicked);
        UpdateUI();
    }

    public void UpdateUI()
    {
        nameText.text = data.upgradeDisplayName;
        levelText.text = "Lv " + data.upgradeLevel;
        costText.text  = data.upgradeCost.Format();
        icon.sprite    = data.sprite;

        CoinAmount nextGen = data.CalculateNextGenerationAmount();
        var delta = CoinAmount.Subtract(nextGen, data.generationAmount).Format();
        string suffix = data.upgradeType == UpgradeType.Energy ? " Energy!" : " Exp!";
        descText.text  = "+" + delta + suffix;
    }

    private void OnBuyClicked()
    {
        menu.TryBuySpecific(data, this);
    }
}