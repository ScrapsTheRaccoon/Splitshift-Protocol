using UnityEngine;

public class BouncePad : Interactable
{
    [SerializeField] private float _bounceForce;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _pressedSprite;

    protected SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _defaultSprite;
    }

    public override void Interact(GameObject player)
    { 
        _rb = player.GetComponent<Rigidbody2D>();
        AudioManager.Instance.PlaySFX(clip);
        _rb.velocity = new Vector2(_rb.velocity.x, _bounceForce);
        _spriteRenderer.sprite = _pressedSprite;
    }

    public void ResetBouncepad()
    {
        _spriteRenderer.sprite = _defaultSprite;
    }
}
