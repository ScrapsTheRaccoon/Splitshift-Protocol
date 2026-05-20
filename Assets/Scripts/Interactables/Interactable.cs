using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected AudioClip clip;

    public abstract void Interact(GameObject initiator);
}
