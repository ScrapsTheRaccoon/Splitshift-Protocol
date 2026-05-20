using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool causesRespawn = true;
    [SerializeField] private string animTrigger = "hurt2";
    public virtual void Damage(GameObject playerObj)
    {
        var health = playerObj.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.Damage(damage, animTrigger, causesRespawn);
    }

    public float GetDamage() => damage;
    public bool CausesRespawn() => causesRespawn;
    public string GetAnimTrigger() => animTrigger;
}
