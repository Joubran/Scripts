using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpgradeZone : MonoBehaviour {
    public int upgradeIndex;
    private UpgradeManager upgradeManager;
    public GameObject popupPrefab;

    void Awake() {
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    void Update() {
        var data = upgradeManager.upgrades[upgradeIndex];
        // only show if not obtained and player has enough coins
        if (!data.isObtained && CoinManager.Instance.HasCoins(data.upgradeCost))
        {
            var popup = Instantiate(popupPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            popup.GetComponent<UpgradePopup>().Initialize(upgradeIndex, data, upgradeManager);
            GetComponent<UpgradeZone>().enabled = false;
        }
    }
}