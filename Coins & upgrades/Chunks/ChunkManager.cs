using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField]
    private List<ChunkData> ChunkList;

    [SerializeField]
    private int _currentLevel = 0;

    // Event that gets called when CurrentLevel changes
    public event Action<int> OnLevelChanged = delegate { };

    /// <summary>
    /// Public property for level. Assignments here fire OnLevelChanged.
    /// </summary>
    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            //Debug.Log($"[ChunkManager] Setting level to {value} (was {_currentLevel})");
            if (_currentLevel != value)
            {
                _currentLevel = value;
                //Debug.Log("[ChunkManager] Level changed, invoking OnLevelChanged");
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }
    }

    private void Awake()
    {
        // Subscribe to own event for debugging
        OnLevelChanged += HandleLevelChanged;

        // Kick off initial change to notify subscribers of starting level
        CurrentLevel = _currentLevel;
    }

    private void Start()
    {
        // Spawn initial chunks up to CurrentLevel
        for (int i = 0; i < ChunkList.Count && i < CurrentLevel; i++)
        {
            SpawnChunk(ChunkList[i]);
        }
    }

    private void SpawnChunk(ChunkData data)
    {
        Instantiate(data.chunkPrefab, data.position, quaternion.identity);
    }

    private void HandleLevelChanged(int newLevel)
    {
        Debug.Log($"[ChunkManager] Level changed to: {newLevel}");
        if(newLevel < ChunkList.Count + 1)
        SpawnChunk(ChunkList[newLevel - 1]);
    }

    /// <summary>
    /// Example method showing how to change level at runtime
    /// </summary>
    public void IncreaseLevel()
    {
        if (CurrentLevel + 1 > ChunkList.Count)
            return;
        CurrentLevel++;
    }
}
