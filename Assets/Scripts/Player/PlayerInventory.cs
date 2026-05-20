using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip useItemClip;
    public void AddJam(JamData jam)
    {
        AudioManager.Instance.PlaySFX(collectClip);
        InventoryManager.Instance.AddJam(jam);
    }

    public bool UseJam(PlayerHealth playerHealth)
    {
        AudioManager.Instance.PlaySFX(useItemClip);
        return InventoryManager.Instance.UseJam(playerHealth);
    }

    public int GetTotalJamCount()
    {
        return InventoryManager.Instance.GetTotalJamCount();
    }

    public event Action<int> onJamUpdated
    {
        add
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.onJamUpdated += value;
        }
        remove
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.onJamUpdated -= value;
        }
    }

}
