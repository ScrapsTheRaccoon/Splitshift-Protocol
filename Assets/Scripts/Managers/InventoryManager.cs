using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<JamData, int> _jamInventory = new();

    public event Action<int> onJamUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddJam(JamData jam)
    {
        if (!_jamInventory.ContainsKey(jam))
        {
            _jamInventory[jam] = 0;
        }

        _jamInventory[jam]++;
        onJamUpdated?.Invoke(GetTotalJamCount());
    }

    public bool UseJam(PlayerHealth playerHealth)
    {
        foreach (var entry in _jamInventory)
        {
            if (entry.Value <= 0) continue;
            if (playerHealth.health >= playerHealth.maxHealth) return false;

            playerHealth.AddHealth(entry.Key.healthPoints);
            _jamInventory[entry.Key]--;
            onJamUpdated?.Invoke(GetTotalJamCount());
            return true;
        }

        return false;
    }

    public int GetTotalJamCount()
    {
        int count = 0;
        foreach (var kv in _jamInventory) count += kv.Value;
        return count;
    }
}
