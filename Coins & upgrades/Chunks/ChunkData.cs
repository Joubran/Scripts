using UnityEngine;

[CreateAssetMenu(menuName = "Clicker/ChunkData")]
public class ChunkData : ScriptableObject
{
    public ChunkType type;
    public GameObject chunkPrefab;
    public Vector3 position;
    public CoinAmount upgradeCost;
}
