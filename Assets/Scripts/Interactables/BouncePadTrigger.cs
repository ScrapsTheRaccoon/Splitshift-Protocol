using UnityEngine;

public class BouncePadTrigger : MonoBehaviour
{
    private BouncePad _bouncePad;

    void Start()
    {
        _bouncePad = GetComponentInParent<BouncePad>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _bouncePad.Interact(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _bouncePad.ResetBouncepad();
        }
    }


}
