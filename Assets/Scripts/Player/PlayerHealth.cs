using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHealth;

    [Header("I-Frames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] public int numberOfFlashes;

    [SerializeField] private GameObject regenParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private ScreenShake screenShake;
    private SpriteRenderer sr;
    private bool isInvulnerable = false;

    public float maxHealth => _maxHealth;
    public float health { get; private set; }
    public bool IsDead { get; private set; } = false;

    private PlayerAnimator _playerAnimator;
    private Player player;

    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        sr = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();
        health = maxHealth;
    }

    public void Damage(float damage, string trigger, bool causesRespawn)
    {
        if (isInvulnerable) return;

        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health > 0)
        {
            _playerAnimator.PlayHurt(trigger);
            AudioManager.Instance.PlaySFX(hurtClip);

            if (causesRespawn)
            {
                player.HandleRespawn();
            }
            else
            {
                //knockback
                StartCoroutine(Invulnerability(iFramesDuration));
                StartCoroutine(Blink(iFramesDuration, numberOfFlashes));
            }
            
            screenShake.ShakeCamera(1f, 1f);
            
        }
        else if (!IsDead)
        {
            AudioManager.Instance.PlaySFX(deathClip);
            IsDead = true;
        }
    }

    public IEnumerator Invulnerability(float duration)
    {
        isInvulnerable = true;
        Physics.IgnoreLayerCollision(7, 8, true);

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator Blink(float blinkDuration, int flashes)
    {
        float flashTime = blinkDuration / (flashes * 2);

        for (int i = 0; i < flashes; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(flashTime);
            sr.enabled = true;
            yield return new WaitForSeconds(flashTime);
        }

        Physics.IgnoreLayerCollision(7, 8, false);
        isInvulnerable = false;
    }

    public void AddHealth(float healthPoints)
    {
        health = Mathf.Clamp(health +  healthPoints, 0, maxHealth);
        var particles = Instantiate(regenParticles, transform.position, Quaternion.identity);
        particles.transform.SetParent(transform);
    }

    public void SetHealth(float value)
    {
        health = Mathf.Clamp(value, 0, maxHealth);
    }

    public float GetHealth()
    {
        return health;
    }
}
