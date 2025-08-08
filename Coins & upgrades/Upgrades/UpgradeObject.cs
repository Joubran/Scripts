using UnityEngine;

public class UpgradeObject : MonoBehaviour
{
    [Header("References")]
    public Collectable collectable;
    public enum ObjectType { Generator, Building };
    public UpgradeData data;
    void Awake()
    {
        collectable = GetComponentInChildren<Collectable>();

        if (collectable == null)
            Debug.Log("[UpgradeObject.cs] CollectableScript not found!!!");
    }

    public void UpdateGenerationAmount() 
    {
        collectable.amountToCollect = data.generationAmount;
    }
}
