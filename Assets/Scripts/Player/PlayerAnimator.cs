using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateAnimator(bool isGrounded, bool isWalking)
    {
        if (isDead) return;

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        
        if (isWalking && isGrounded)
        {
            resetJumpTriggers();
        }
    }

    public void PlayJump()
    {
        if (isDead) return;
        anim.SetTrigger("jump");
    }

    public void PlayFall()
    {
        if (isDead) return;
        anim.SetTrigger("fall");
    }

    public void PlayLand()
    {
        if (isDead) return;
        anim.SetTrigger("land");
    }

    public void PlayHurt(string trigger)
    {
        if (isDead) return;
        if (trigger == "hurt") { anim.SetTrigger("hurt"); }
        else if (trigger == "hurt2") { anim.SetTrigger("hurt2"); }
    }

    public void PlayDeath()
    {
        if (isDead) return;
        anim.SetTrigger("death");
        isDead = true;
    }

    private void resetJumpTriggers()
    {
        anim.ResetTrigger("jump");
        anim.ResetTrigger("fall");
        anim.ResetTrigger("land");
    }
}
