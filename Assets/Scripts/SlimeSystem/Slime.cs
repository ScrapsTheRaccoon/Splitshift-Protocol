using UnityEngine;

public class Slime : MonoBehaviour
{

    [SerializeField] private bool registerOnStart = false;
    [SerializeField] private AudioClip invalidActionClip;

    public enum SlimeSize { Small, Medium, Large }
    public SlimeSize size;

    public bool isActivePlayer = false;

    private SlimeManager manager;
    private PlayerEmotionBubble activeEmotionBubble;
    private ScreenShake screenShake;

    private void Start()
    {
        activeEmotionBubble = GetComponentInChildren<PlayerEmotionBubble>();
        manager = FindAnyObjectByType<SlimeManager>();
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();

        if (registerOnStart)
        {
            FindAnyObjectByType<SlimeManager>().RegisterSlime(this);
            GetComponent<PlayerMoisture>().Init(true);
        }
    }

    private void TriggerInvalidAction()
    {
        AudioManager.Instance.PlaySFX(invalidActionClip);
        screenShake.ShakeCamera(1f, 0.7f);

        if (activeEmotionBubble != null)
        {
            activeEmotionBubble.TriggerInvalidWarning();
        }
    }

    public void RequestSplit()
    {
        if (CanSplit())
        {
            manager.Split(this);
        }
        else
        {
            TriggerInvalidAction();
        }
    }

    public void RequestMerge()
    {
        if (!isActivePlayer) return;

        Slime otherSlime = FindTouchingSlime();
        if (otherSlime != null && CanMerge(this.size, otherSlime.size))
        {
            manager.Merge(this, otherSlime);
        }
        else
        {
            TriggerInvalidAction();
        }
    }

    public bool CanMerge(Slime.SlimeSize sizeA, Slime.SlimeSize sizeB)
    {
        if (sizeA == SlimeSize.Large || sizeB == SlimeSize.Large) return false;
        if (sizeA == SlimeSize.Medium && sizeB == SlimeSize.Medium) return false;

        return (sizeA == Slime.SlimeSize.Small && sizeB == Slime.SlimeSize.Small) ||
           (sizeA == Slime.SlimeSize.Medium && sizeB == Slime.SlimeSize.Small) ||
           (sizeA == Slime.SlimeSize.Small && sizeB == Slime.SlimeSize.Medium);
    }

    private Slime FindTouchingSlime()
    {
        float mergeRange = 0.75f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, mergeRange);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            Slime slime = hit.GetComponent<Slime>();
            if (slime != null)
            {
                return slime;
            }
        }

        return null;
    }

    public void HandleDeath()
    {
        manager.OnPlayerDeath(this);
    }

    public (SlimeStats first, SlimeStats second) GetSplitValues()
    {
        float currentHealth = GetComponent<PlayerHealth>().GetHealth();
        float currentMoisture = GetComponent<PlayerMoisture>().GetMoisture();

        SlimeStats first = new SlimeStats();
        SlimeStats second = new SlimeStats();

        switch (size)
        {
            case SlimeSize.Large:
                float totalHealthL = 6f;
                float totalMoistureL = 30f;

                float missingHealthL = totalHealthL - currentHealth;
                float missingMoistureL = totalMoistureL - currentMoisture;

                // Target values
                first.health = 4f;
                second.health = 2f;
                first.moisture = 20f;
                second.moisture = 10f;

                // Subtract penalty from first (Medium)
                first.health = Mathf.Max(0, first.health - missingHealthL);
                first.moisture = Mathf.Max(0, first.moisture - missingMoistureL);
                break;

            case SlimeSize.Medium:
                float totalHealthM = 4f;
                float totalMoistureM = 20f;

                float missingHealthM = totalHealthM - currentHealth;
                float missingMoistureM = totalMoistureM - currentMoisture;

                // Each small gets 2 health, 10 moisture
                first.health = 2f;
                second.health = 2f;
                first.moisture = 10f;
                second.moisture = 10f;

                // Penalty to first small
                first.health = Mathf.Max(0, first.health - missingHealthM);
                first.moisture = Mathf.Max(0, first.moisture - missingMoistureM);
                break;
        }

        return (first, second);
    }


    public float GetHealth() => GetComponent<PlayerHealth>().GetHealth();
    public float GetMoisture() => GetComponent<PlayerMoisture>().GetMoisture();

    public bool CanSplit()
    {
        if (size == SlimeSize.Small) return false;

        float health = GetHealth();
        return size switch
        {
            SlimeSize.Large => health >= 3f,
            SlimeSize.Medium => health >= 2f,
            _ => false
        };
    }

    public float GetCombinedHealth(Slime other)
    {
        return this.GetHealth() + other.GetHealth();
    }

    public float GetCombinedMoisture(Slime other)
    {
        return this.GetMoisture() + other.GetMoisture();
    }

    public void Init(float health, float moisture)
    {
        GetComponent<PlayerHealth>().SetHealth(health);
        GetComponent<PlayerMoisture>().SetMoisture(moisture);
    }


    // DEBUG FOR FINDING ACTIVE SLIME
    void OnDrawGizmos()
    {
        if (isActivePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

        }
    }

}
