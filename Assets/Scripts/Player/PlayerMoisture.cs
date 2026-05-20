using System.Collections;
using UnityEngine;

public class PlayerMoisture : MonoBehaviour
{
    [SerializeField] private float maxMoisture;
    public float MaxMoisture => maxMoisture;
    public float moisture { get; private set; }

    [SerializeField] private float dehydrationRate;
    [SerializeField] private float hydrationRate;

    [SerializeField] private AudioClip hydrateClip;
    [SerializeField] private AudioClip enterWaterClip;
    [SerializeField] private AudioClip exitWaterClip;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private PlayerEmotionBubble emotionBubble;
    private Slime slime;
    private SlimeManager slimeManager;

    private Coroutine activeCoroutine;
    private bool wasInWater;

    private float timeSinceLastDamage = 0f;
    private float damageInterval = 2f;

    public void Init(SlimeManager manager)
    {
        slimeManager = manager;
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        emotionBubble = GetComponentInChildren<PlayerEmotionBubble>();
        slime = GetComponent<Slime>();

        wasInWater = playerMovement.inWater;
        activeCoroutine = StartCoroutine(wasInWater ? Hydrate() : Dehydrate());
    }

    void Update()
    {
        bool currentlyInWater = playerMovement.inWater;

        if (wasInWater != currentlyInWater)
        {
            wasInWater = currentlyInWater;

            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);

            if (currentlyInWater)
            {
                AudioManager.Instance.PlaySFX(enterWaterClip);
                AudioManager.Instance.PlayLoopingSFX(hydrateClip);
                activeCoroutine = StartCoroutine(Hydrate());
            }
            else
            {
                AudioManager.Instance.PlaySFX(exitWaterClip);
                AudioManager.Instance.StopLoopingSFX();
                activeCoroutine = StartCoroutine(Dehydrate());
            }

            emotionBubble.ShowIdle();
        }
    }

    public void SetMoisture(float value)
    {
        moisture = Mathf.Clamp(value, 0, MaxMoisture);
    }

    public float GetMoisture()
    {
        return moisture;
    }

    public void Init(bool startAtMax = false)
    {
        if (startAtMax)
        {
            moisture = MaxMoisture;
        }
    }

    private IEnumerator Dehydrate()
    {
        while (true)
        {
            if (moisture > 0)
            {
                moisture -= 1;
                moisture = Mathf.Clamp(moisture, 0, MaxMoisture);
                timeSinceLastDamage = 0f;

                if (moisture <= 2)
                {
                    if (slime.isActivePlayer)
                    {
                        emotionBubble.ShowDrying();
                    }
                    else
                    {
                        slimeManager?.TriggerWarningOnActive();
                    }
                }
            }
            else
            {
                // TO-DO: add SFX or screen shake???
                if (slime.isActivePlayer)
                {
                    emotionBubble.ShowDrying();
                }
                else
                {
                    slimeManager?.TriggerWarningOnActive();
                }

                timeSinceLastDamage += dehydrationRate;
                if (timeSinceLastDamage >= damageInterval)
                {
                    playerHealth.Damage(1, "hurt", false);
                    timeSinceLastDamage = 0f;
                }
            }

            yield return new WaitForSeconds(dehydrationRate);
        }
    }

    private IEnumerator Hydrate()
    {
        while (true)
        {
            if (moisture < MaxMoisture)
            {
                emotionBubble.ShowHydrating();
                moisture += 1;
                moisture = Mathf.Clamp(moisture, 0, MaxMoisture);
            }
            else if (moisture >= 5 && slime.isActivePlayer)
            {
                emotionBubble.ShowIdle();
            }

            yield return new WaitForSeconds(hydrationRate);
        }
    }
}
