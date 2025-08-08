using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<UpgradeData> upgrades;
    public List<UpgradeObject> upgradeObjects;

    void Start()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].isObtained)
            {
                upgradeObjects.Add(SpawnUpgrade(i));
            }
        }


    }

    UpgradeObject SpawnUpgrade(int upgradeIndex)
    {
        // spawn the prefab
        GameObject prefab = Instantiate(upgrades[upgradeIndex].upgradePrefab, upgrades[upgradeIndex].position, upgrades[upgradeIndex].upgradePrefab.transform.rotation);

        UpgradeObject upgradeObject = prefab.GetComponent<UpgradeObject>();

        upgradeObject.data = upgrades[upgradeIndex];
        upgradeObject.UpdateGenerationAmount();

        return upgradeObject;
    }
    
    public void SpawnUpgradeAt(int index) {
        if (!upgrades[index].isObtained) {
            upgrades[index].isObtained = true;
            upgradeObjects.Add(SpawnUpgrade(index));
        }
    }
}
