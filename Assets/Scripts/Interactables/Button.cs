using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _pressedSprite;

    private SpriteRenderer _renderer;

    private bool _isPressed = false;

    public UnityEvent onPressed;
    public UnityEvent onPressReleased;
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if ( _renderer != null)
        {
            _renderer.sprite = _defaultSprite;
        }
    }

    public override void Interact(GameObject initiator)
    {
        if (_isPressed) return;

        _isPressed = true;
        AudioManager.Instance.PlaySFX(clip);
        _renderer.sprite = _pressedSprite;
        onPressed?.Invoke();
        
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (_isPressed && other.gameObject.CompareTag("Player"))
        {
            _isPressed = false;
            AudioManager.Instance.PlaySFX(clip);
            ResetButton();
            onPressReleased?.Invoke();
        }
    }

    public void ResetButton()
    {
        _renderer.sprite = _defaultSprite;
    }
}
