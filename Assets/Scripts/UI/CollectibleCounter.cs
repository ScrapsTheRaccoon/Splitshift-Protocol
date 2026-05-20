using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectibleCounter : MonoBehaviour
{
    [SerializeField] private Image collectibleIcon;
    [SerializeField] private Text counter;
    [SerializeField] private Text counterShadow;
    private PlayerInventory inventory;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryBindInventory();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (inventory != null)
            inventory.onJamUpdated -= updateCounter;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBindInventory();
    }

    private void TryBindInventory()
    {
        if (InventoryManager.Instance == null)
            return;

        var foundInventory = FindAnyObjectByType<PlayerInventory>();
        if (foundInventory != null)
        {
            if (inventory != null)
                inventory.onJamUpdated -= updateCounter;

            inventory = foundInventory;
            inventory.onJamUpdated += updateCounter;
            updateCounter(inventory.GetTotalJamCount());
        }
    }

    void updateCounter(int collectibleCount)
    {
        counter.text = "x" + collectibleCount;
        counterShadow.text = counter.text;
    }
}

