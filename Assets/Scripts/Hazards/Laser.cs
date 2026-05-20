using System.Collections;
using UnityEngine;

public class Laser : Hazard
{
    [Header("Laser Timer Properties")]
    [SerializeField] private float flashTimer;
    [SerializeField] private float offTimer;
    [SerializeField] private float flashDelay;

    [Header("Laser Audio Properties")]
    [SerializeField] private AudioClip AudioClip;

    private bool hasDelayed = false;
    private Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();

        if (flashTimer > 0)
        {
            StartCoroutine(FlashLoop());
        }
        else
        {
            anim.SetBool("isActive", true);
        }
    }

    public void TurnOffLaser()
    {
        anim.SetBool("isActive", false);
    }

    public void TurnOnLaser()
    {
        anim.SetBool("isActive", true);
    }

    private IEnumerator FlashLoop()
    {
        while (true)
        {
            if (!hasDelayed)
            {
                yield return new WaitForSeconds(flashDelay);
                hasDelayed = true;
            }

            anim.SetBool("isActive", true);
            yield return new WaitForSeconds(flashTimer);

            anim.SetBool("isActive", false);
            yield return new WaitForSeconds(offTimer);
        }

    }

}
