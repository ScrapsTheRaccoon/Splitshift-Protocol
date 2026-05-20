using UnityEngine;

public class PlayerEmotionBubble : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ShowHydrating()
    {
        SetAllFalse();
        anim.SetBool("isHydrating", true);
    }

    public void ShowDrying()
    {
        SetAllFalse();
        anim.SetBool("isDryingOut", true);
    }

    public void ShowIdle()
    {
        SetAllFalse();
        anim.SetBool("isIdle", true);
    }

    public void ShowWarning()
    {
        SetAllFalse();
        anim.SetTrigger("checkOnThem");
    }

    public void Communicate()
    {
        // play a communication anim
    }

    private void SetAllFalse()
    {
        anim.SetBool("isHydrating", false);
        anim.SetBool("isDryingOut", false);
        anim.SetBool("isIdle", false);
    }

    public void TriggerInvalidWarning()
    {
        SetAllFalse();
        anim.SetTrigger("invalidAction");
    }
}
