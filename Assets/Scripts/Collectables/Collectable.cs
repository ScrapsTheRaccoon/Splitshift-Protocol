using UnityEngine;

public abstract class Collectable : Interactable
{
    [SerializeField] private float _hoverSpeed;
    [SerializeField] private float _hoverHeight;

    private Vector3 _startPos;


    protected abstract void Collect(GameObject initiator);

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        Hover();
    }

    private void Hover()
    {
        float offset = Mathf.Sin(Time.time * _hoverSpeed) * _hoverHeight;
        transform.position = _startPos + new Vector3(0f, offset, 0f);
    }

    public override void Interact(GameObject player)
    {
        Collect(player);
        Destroy(gameObject);
    }
}
