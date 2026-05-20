using UnityEngine;
using UnityEngine.Events;

public class Lever : Interactable
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _pulledSprite;

    private bool _pulled = false;

    private SpriteRenderer _renderer;
    private Animator _anim;
    private Collider2D _collider;

    public UnityEvent onLeverPulled;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _renderer.sprite = _defaultSprite;
    }

    public override void Interact(GameObject initiator)
    {
        if (_pulled) return;

        _pulled = true;
        AudioManager.Instance.PlaySFX(clip);
        _renderer.sprite = _pulledSprite;
        _anim.SetTrigger("Flip");
        _collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interact(other.gameObject);
    }
}
