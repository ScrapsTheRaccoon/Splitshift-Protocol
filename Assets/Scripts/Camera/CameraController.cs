using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // === CAMERA FOLLOW SETTINGS ===
    [Header("Follow Behavior")]
    [SerializeField] private float _yFollowThreshold = 1f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _offset = 2f;
    [SerializeField] private float _offsetSmoothing = 0.2f;

    // === CAMERA ZOOM DYNAMICS ===
    [Header("Zoom Based on Distance")]
    [SerializeField] private float farDistanceThreshold = 10f;
    [SerializeField] private float mediumDistanceThreshold = 5f;
    [SerializeField] private float farDistanceMultiplier = 4f;
    [SerializeField] private float mediumDistanceMultiplier = 2f;

    // === CAMERA LIMITS ===
    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minCameraBounds = new Vector2(0, 0);
    [SerializeField] private Vector2 maxCameraBounds = new Vector2(0, 0);

    [Header("DEBUGGING SHIT")]

    private bool isTemporarilyFocusing = false;

    [SerializeField] private Vector3 _targetPos;
    private Transform _player;
    private SlimeManager _manager;

    void Start()
    {
        _manager = FindAnyObjectByType<SlimeManager>();
        _player = _manager.GetActiveSlimeTransform();
        _targetPos = _player.transform.position;
    }

    void LateUpdate()
    {
        if (isTemporarilyFocusing) return;

        _player = _manager.GetActiveSlimeTransform();
        if (_player == null) return;

        float targetX = _player.position.x;
        float targetY = transform.position.y;
        float targetZ = transform.position.z;

        if (Mathf.Abs(_player.position.y - transform.position.y) > _yFollowThreshold)
        {
            targetY = Mathf.Lerp(transform.position.y, _player.position.y, Time.deltaTime * _followSpeed);
        }

        if (_player.localScale.x > 0f)
        {
            targetX += _offset;
        }
        else
        {
            targetX -= _offset;
        }

        _targetPos = new Vector3(targetX, targetY, targetZ);

        _targetPos.x = Mathf.Clamp(targetX, minCameraBounds.x, maxCameraBounds.x);
        _targetPos.y = Mathf.Clamp(targetY, minCameraBounds.y, maxCameraBounds.y);

        float distance = Vector3.Distance(transform.position, _targetPos);

        float dynamicSmoothing = _offsetSmoothing;

        if (distance > farDistanceThreshold)
        {
            dynamicSmoothing *= farDistanceMultiplier;
        }
        else if (distance > mediumDistanceThreshold)
        {
            dynamicSmoothing *= mediumDistanceMultiplier;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPos, dynamicSmoothing * Time.deltaTime);
    }

    public void FocusOnTemporaryTarget(Transform target, float duration = 3.0f)
    {
        StartCoroutine(FocusRoutine(target, duration));
    }

    private IEnumerator FocusRoutine(Transform target, float duration)
    {
        if (target == null) yield break;

        isTemporarilyFocusing = true;

        float originalSize = Camera.main.orthographicSize;
        float zoomedInSize = originalSize * 0.75f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null) break;

            float xOffset = target.localScale.x > 0f ? _offset : -_offset;
            float targetX = Mathf.Clamp(target.position.x + xOffset, minCameraBounds.x, maxCameraBounds.x);
            float targetY = Mathf.Clamp(target.position.y, minCameraBounds.y, maxCameraBounds.y);

            Vector3 focusPos = new Vector3(targetX, targetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, focusPos, 10f * Time.deltaTime);

            Camera.main.orthographicSize = Mathf.Lerp(originalSize, zoomedInSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.orthographicSize = originalSize;
        _player = _manager.GetActiveSlimeTransform();
        isTemporarilyFocusing = false;
    }

}
