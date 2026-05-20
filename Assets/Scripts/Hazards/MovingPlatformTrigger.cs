using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Getting Squished");
            var health = other.gameObject.GetComponent<PlayerHealth>();
            var movement = other.gameObject.GetComponent<PlayerMovement>();

            if(movement != null && movement.isGrounded)
            {
                health.Damage(1, "hurt2", true);
            }
        }
    }
}
