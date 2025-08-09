using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpgradeZone : MonoBehaviour {
    public int upgradeIndex;
    private UpgradeManager upgradeManager;
    public GameObject popupPrefab;
    private UpgradeData data;

    void Awake() {
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    void OnEnable() {
        data = upgradeManager.upgrades[upgradeIndex];
        CoinManager.Instance.CurrencyChanged += OnCurrencyOrPriceChanged;
        data.CostChanged += OnCurrencyOrPriceChanged;
        CheckAffordable();
    }

    void OnDisable() {
        if (CoinManager.Instance != null)
            CoinManager.Instance.CurrencyChanged -= OnCurrencyOrPriceChanged;
        if (data != null)
            data.CostChanged -= OnCurrencyOrPriceChanged;
    }

    private void OnCurrencyOrPriceChanged() {
        CheckAffordable();
    }

    private void OnCurrencyOrPriceChanged(CoinAmount _) {
        CheckAffordable();
    }

    private void CheckAffordable() {
        if (!data.isObtained && CoinManager.Instance.HasCoins(data.upgradeCost)) {
            var popup = Instantiate(popupPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            popup.GetComponent<UpgradePopup>().Initialize(upgradeIndex, data, upgradeManager);
            enabled = false;
        }
    }
}

