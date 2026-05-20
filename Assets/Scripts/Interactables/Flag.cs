using System.Collections;
using UnityEngine;

public class Flag : Interactable
{
    [SerializeField] private GameObject exitTarget;
    [SerializeField] private float completeLevelDelay;

    public override void Interact(GameObject player)
    {
        PlayerMovement movement = player.GetComponent <PlayerMovement>();

        Vector3 target = exitTarget.transform.position;
        movement.OnLevelComplete(target);

        AudioManager.Instance.PlaySFX(clip);
        StartCoroutine(CompleteAfterDelay(completeLevelDelay));
    }

    private IEnumerator CompleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.CompleteLevel();
    }
}
