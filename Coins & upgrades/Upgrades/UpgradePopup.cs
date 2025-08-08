using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePopup : MonoBehaviour {
    //public TMP_Text nameText;
    public Image iconImage;
    public Button purchaseButton;
    public TMP_Text buttonText;

    private int index;
    private UpgradeData data;
    private UpgradeManager manager;

    public void Initialize(int upgradeIndex, UpgradeData upgradeData, UpgradeManager um) {
        index = upgradeIndex;
        data = upgradeData;
        manager = um;

        // fill UI
        //nameText.text = data.upgradeDisplayName;
        iconImage.sprite = data.sprite;
        buttonText.text = CoinManager.Instance.GetFormattedCoins(data.upgradeCost);

        purchaseButton.onClick.AddListener(OnPurchaseClicked);
    }

    private void OnPurchaseClicked() {
        if (CoinManager.Instance.UseCoins(data.upgradeCost)) {
            manager.SpawnUpgradeAt(index);
            Destroy(gameObject);
        } else {
            // optional: shake or flash to indicate insufficient funds
            Debug.Log("Not enough coins!");
        }
    }
}
