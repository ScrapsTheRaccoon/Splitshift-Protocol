using UnityEngine;
using UnityEngine.UI;
using static Slime;

public class StatusBarController : MonoBehaviour
{
    [Header("Health Bar Prefabs")]
    [SerializeField] private GameObject largeHealthBarPrefab;
    [SerializeField] private GameObject mediumHealthBarPrefab;
    [SerializeField] private GameObject smallHealthBarPrefab;

    [Header("Moisture Bar Prefabs")]
    [SerializeField] private GameObject largeMoistureBarPrefab;
    [SerializeField] private GameObject mediumMoistureBarPrefab;
    [SerializeField] private GameObject smallMoistureBarPrefab;

    [Header("Current Lives Text")]
    [SerializeField] private Text lives;
    [SerializeField] private Text livesShadow;

    [Header("Active Slimes Text")]
    [SerializeField] private Text slimeCount;
    [SerializeField] private Text slimeCountShadow;

    private Slime activeSlime;
    private PlayerHealth activeHealth;
    private PlayerMoisture activeMoisture;

    private Image activeHealthBar;
    private Image activeMoistureBar;

    private int slimes;

    private void Update()
    {
        if (activeSlime != null && activeHealth != null && activeMoisture != null)
        {
            UpdateStatusBars();
        }
    }

    public void UpdateUIWithActiveSlime(Slime slime, int slimeCount)
    {
        activeSlime = slime;
        slimes = slimeCount;

        activeHealth = slime.GetComponent<PlayerHealth>();
        activeMoisture = slime.GetComponent<PlayerMoisture>();

        // Disable all health bars
        largeHealthBarPrefab.SetActive(false);
        mediumHealthBarPrefab.SetActive(false);
        smallHealthBarPrefab.SetActive(false);

        // Disable all moisture bars
        largeMoistureBarPrefab.SetActive(false);
        mediumMoistureBarPrefab.SetActive(false);
        smallMoistureBarPrefab.SetActive(false);

        // Activate the correct bars and cache the Image components
        switch (slime.size)
        {
            case Slime.SlimeSize.Large:
                largeHealthBarPrefab.SetActive(true);
                largeMoistureBarPrefab.SetActive(true);
                activeHealthBar = largeHealthBarPrefab.transform.Find("HealthBarTotal")?.GetComponent<Image>();
                activeMoistureBar = largeMoistureBarPrefab.transform.Find("MoistureBarTotal")?.GetComponent<Image>();
                break;

            case Slime.SlimeSize.Medium:
                mediumHealthBarPrefab.SetActive(true);
                mediumMoistureBarPrefab.SetActive(true);
                activeHealthBar = mediumHealthBarPrefab.transform.Find("HealthBarTotal")?.GetComponent<Image>();
                activeMoistureBar = mediumMoistureBarPrefab.transform.Find("MoistureBarTotal")?.GetComponent<Image>();
                break;

            case Slime.SlimeSize.Small:
                smallHealthBarPrefab.SetActive(true);
                smallMoistureBarPrefab.SetActive(true);
                activeHealthBar = smallHealthBarPrefab.transform.Find("HealthBarTotal")?.GetComponent<Image>();
                activeMoistureBar = smallMoistureBarPrefab.transform.Find("MoistureBarTotal")?.GetComponent<Image>();
                break;
        }

        if (activeSlime != null)
        {
            UpdateStatusBars();
        }
    }

    private void UpdateStatusBars()
    {
        float healthPercent = 1f - (activeHealth.health / activeHealth.maxHealth);
        float moisturePercent = 1f - (activeMoisture.moisture / activeMoisture.MaxMoisture);

        if (activeHealthBar != null)
            activeHealthBar.fillAmount = Mathf.Clamp01(healthPercent);

        if (activeMoistureBar != null)
            activeMoistureBar.fillAmount = Mathf.Clamp01(moisturePercent);

        string healthText = $"{(int)activeHealth.health}/{(int)activeHealth.maxHealth}";
        lives.text = healthText;
        livesShadow.text = healthText;

        slimeCount.text = "x" + slimes;
        slimeCountShadow.text = slimeCount.text;
    }
}
