using UnityEngine;

public class Console : Interactable
{
    [SerializeField] private bool startsOn;
    [SerializeField] private bool turnsOn;
    [SerializeField] private bool turnsOff;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        if(anim != null && startsOn)
        {
            anim.SetTrigger("turnOn");
        }
    }

    public override void Interact(GameObject player)
    {
        if (player != null && turnsOn)
        {
            anim.SetTrigger("turnOn");
        }
        else if (player != null && turnsOff)
        {
            anim.SetTrigger("turnOff");
        }
    }
}
