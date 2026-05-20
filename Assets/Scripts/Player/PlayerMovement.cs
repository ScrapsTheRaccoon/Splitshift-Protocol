using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxAirSpeed;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _waterMask;

    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;

    [SerializeField] private float walkSoundInterval = 0.5f;
    private float walkTimer;

    private Rigidbody2D _rb;
    private Vector2 lastPosition;

    private float horizontalInput;
    private bool jumpPressed;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    public float VerticalVelocity => _rb.velocity.y;

    public bool isGrounded { get; private set; }
    public bool isMoving { get; private set; }
    public bool inWater { get; private set; }
    private bool isActivePlayer;

    private bool levelComplete = false;
    private Vector2 autoWalkDirection;
    [SerializeField] private float autoWalkSpeed = 5f;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void ReceiveInput(float horizontal, bool _jumpPressed, bool _isActivePlayer, bool _isDead)
    {
        horizontalInput = horizontal;
        jumpPressed = _jumpPressed;
        isActivePlayer = _isActivePlayer;

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (_isDead)
        {
            _rb.bodyType = RigidbodyType2D.Static;
        }
    }

    void Update()
    {
        if (isGrounded && isMoving && isActivePlayer)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= walkSoundInterval)
            {
                AudioManager.Instance.PlaySFX(walkClip);
                walkTimer = 0f;
            }
        }
        else
        {
            walkTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (levelComplete)
        {
            _rb.velocity = autoWalkDirection * autoWalkSpeed;
            return;
        }

        HandleMovement();
        
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }

        HandleJump();

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        Vector2 velocity = (_rb.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = _rb.position;

        inWater = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, _waterMask);
    }

    void HandleMovement()
    {
        _rb.velocity = new Vector2(horizontalInput * speed, _rb.velocity.y);

        Vector3 scale = transform.localScale;

        if (horizontalInput > 0.01f)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (horizontalInput < -0.01f)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;


        if (!isGrounded)
        {
            _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -maxAirSpeed, maxAirSpeed), _rb.velocity.y);
        }

        if (!isActivePlayer)
             _rb.bodyType = RigidbodyType2D.Kinematic; // _rb.drag = 5f;
        else
            _rb.bodyType = RigidbodyType2D.Dynamic; // _rb.drag = 0f;

        isMoving = Mathf.Abs(horizontalInput) > 0.01f;
    }

    public void OnLevelComplete(Vector3 walkDirection)
    {
        levelComplete = true;
        autoWalkDirection = walkDirection.normalized;
        autoWalkSpeed = speed;

        // Disable player input
        horizontalInput = 0;
        jumpPressed = false;

        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0; // Prevent bouncing on steps or slopes
    }

    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, _groundMask);

        if (jumpBufferCounter > 0 && isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            jumpBufferCounter = 0;

            AudioManager.Instance.PlaySFX(jumpClip);
        }
    }

    //debug circle for groundcheck:
    void OnDrawGizmosSelected()
    {
        if (_groundCheck != null)
            Gizmos.DrawWireSphere(_groundCheck.position, groundCheckRadius);
    }

    public void Respawn()
    {
        transform.position = CheckpointManager.Instance.GetCheckpoint();
        _rb.velocity = Vector2.zero; // stop weird velocity carryover
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            CheckpointManager.Instance.SetCheckpoint(other.transform.position);
        }
    }

}
