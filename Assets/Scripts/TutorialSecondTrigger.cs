using UnityEngine;

public class TutorialSecondTrigger : MonoBehaviour
{
    public static bool reached = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        reached = true;
    }
}
