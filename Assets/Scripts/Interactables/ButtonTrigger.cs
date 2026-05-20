using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponentInParent<Button>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _button.Interact(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _button.ResetButton();
        }
    }
}
