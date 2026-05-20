using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField] private float selfDestructTime;
    [SerializeField] private AudioClip audioClip;

    void Start()
    {
        if (audioClip != null)
        {
            AudioManager.Instance.PlaySFX(audioClip);
        }

        Destroy(this.gameObject, selfDestructTime);
    }
}
