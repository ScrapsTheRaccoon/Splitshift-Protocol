using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject respawnFxPrefab;
    [SerializeField] private GameObject deathFxPrefab;
    [SerializeField] private GameObject landFxPrefab;
    [SerializeField] private AudioClip landClip;

    private PlayerMovement movement;
    private PlayerAnimator animator;
    private PlayerHealth health;
    private PlayerInventory inventory;
    private PlayerEmotionBubble emotionBubble;
    private Slime slime;

    private float horizontalInput;
    private bool jumpPressed;
    private bool isActiveSlime;

    private bool wasGrounded;
    private bool leftGround = false;
    private bool isFalling = false;

    private bool isDead = false;
    private bool hasDied = false;
    private bool isHurting = false;

    private Dictionary<KeyCode, Action> keyActions;
    private SpriteRenderer sr;
    private ScreenShake screenShake;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerAnimator>();
        health = GetComponent<PlayerHealth>();
        inventory = GetComponent<PlayerInventory>();
        emotionBubble = GetComponentInChildren<PlayerEmotionBubble>();
        slime = GetComponent<Slime>();

        sr = GetComponent<SpriteRenderer>();
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();


        keyActions = new Dictionary<KeyCode, Action>()
        {
            {KeyCode.R, () => inventory.UseJam(health) },
            {KeyCode.Q, ()=>  slime.RequestSplit() },
            {KeyCode.E, () => slime.RequestMerge() }
        };

    }

    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver))
            return;

        isDead = health.IsDead;

        if (isDead && !hasDied)
        {
            hasDied = true;
            StartCoroutine(DeathSequence());
        }

        isActiveSlime = slime.isActivePlayer;

        SetEmotionBubbleActive(isActiveSlime);
        movement.enabled = isActiveSlime && !isDead && !isHurting;
        slime.enabled = isActiveSlime && !isDead;
        health.enabled = isActiveSlime && !isDead;
        inventory.enabled = isActiveSlime && !isDead;

        if (!hasDied)
        {
            HandleMovementInput();
            HandleActions();
            HandleAnimator();
        }
    }

    private void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);

        movement.ReceiveInput(horizontalInput, jumpPressed, isActiveSlime, isDead);
    }

    private void HandleAnimator()
    {
        if (isDead) return;

        animator.UpdateAnimator(movement.isGrounded, movement.isMoving);

        if (isActiveSlime && jumpPressed && movement.isGrounded)
        {
            animator.PlayJump();
        }

        if (!movement.isGrounded && wasGrounded)
        {
            leftGround = true;

            if (movement.VerticalVelocity < -0.1f)
            {
                isFalling = true;
                animator.PlayFall();
            }
        }

        if (leftGround && movement.VerticalVelocity < -0.1f && !isFalling)
        {
            isFalling = true;
            animator.PlayFall();
        }

        if (leftGround && movement.isGrounded)
        {
            leftGround = false;
            isFalling = false;

            animator.PlayLand();
            Instantiate(landFxPrefab, transform.position, Quaternion.identity, this.transform);

            AudioManager.Instance.PlaySFX(landClip);
            screenShake.ShakeCamera(1f, 0.7f);
        }

        wasGrounded = movement.isGrounded;
    }

    private void HandleActions()
    {
        foreach (var entry in keyActions)
        {
            if (Input.GetKeyDown(entry.Key))
            {
                entry.Value.Invoke();
                break;
            }
        }
    }

    private void SetEmotionBubbleActive(bool isActiveSlime)
    {
        if (emotionBubble != null)
        {
            emotionBubble.gameObject.SetActive(isActiveSlime);
        }
    }

    private IEnumerator DeathSequence()
    {
        slime.HandleDeath();
        animator.PlayDeath();
        yield return new WaitForSeconds(1.2f);

        Destroy(gameObject, 1.5f);
    }

    public void HandleRespawn()
    {
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        isHurting = true;
        movement.enabled = false;

        // Start invulnerability but no blinking, duration is total invuln length (vfx + blink time)
        float totalInvulnDuration = 1.51f + 1.5f; // deathFx + blinking time approx
        StartCoroutine(health.Invulnerability(totalInvulnDuration));

        yield return new WaitForSeconds(0.15f); // delay for hurt anim and vfx
        sr.enabled = false; // player disappears
        emotionBubble.gameObject.SetActive(false);

        Instantiate(deathFxPrefab, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(1.51f); // delay for death vfx

        movement.Respawn();
        Instantiate(respawnFxPrefab, transform.position, Quaternion.identity, transform);

        sr.enabled = true; // player reappears at checkpoint
        emotionBubble.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.01f);

        // Now blink AFTER respawn and vfx
        yield return StartCoroutine(health.Blink(1.5f, health.numberOfFlashes));

        movement.enabled = true;
        isHurting = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Interactable interactable = other.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact(this.gameObject);
            }

        }
        if (other.gameObject.CompareTag("Hazard"))
        {
            Hazard hazard = other.gameObject.GetComponent<Hazard>();  
            if (hazard != null)
            {
                hazard.Damage(this.gameObject);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Interactable interactable = other.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact(this.gameObject);
            }

        }
        if (other.CompareTag("Hazard"))
        {
            var hazard = other.GetComponent<Hazard>() ?? other.GetComponentInParent<Hazard>();
            if (hazard != null)
            {
                hazard.Damage(this.gameObject);
            }
        }
    }


}
