using UnityEngine;

public class PortraitController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private Slime.SlimeSize currentSize;

    public void UpdatePortrait(Slime slime)
    {
        var size = slime.size;

        if (currentSize != size)
        {
            currentSize = size;
            SwapSlime(size);
        }
    }

    public void SwapSlime(Slime.SlimeSize size)
    {
        anim.ResetTrigger("small");
        anim.ResetTrigger("medium");
        anim.ResetTrigger("large");

        anim.SetTrigger(size.ToString().ToLower());
    }
}
