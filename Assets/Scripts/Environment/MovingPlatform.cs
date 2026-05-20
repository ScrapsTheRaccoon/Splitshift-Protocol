using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Vector3 designatedPosition;
    [SerializeField] private float speed = 1f;

    private bool _isRerouted = false;
    private Vector3 _targetPosition;

    private void Start()
    {
        transform.position = startPosition;
        _targetPosition = endPosition;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_isRerouted)
        {
            transform.position = Vector3.MoveTowards(transform.position, designatedPosition, speed * Time.deltaTime);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
        {
            _targetPosition = (_targetPosition == startPosition) ? endPosition : startPosition;
        }
    }

    public void RerouteToDesignatedPosition()
    {
        _isRerouted = true;
    }

    public void ResumeNormalMovement()
    {
        _isRerouted = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }


}
