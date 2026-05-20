using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxTilemap : MonoBehaviour
{
    private Vector2 _startingPos;
    private float _length; // length of tilemap on the x axis
    private Camera _cam;

    [Tooltip("0 = move with camera, 1 = no movement")]
    public float parallaxAmountX;
    public float parallaxAmountY;
    public bool loop = true;

    private TilemapRenderer _tilemapRenderer;
    private Bounds _tilemapBounds;
    private Vector2 _camStartingPosition;

    void Start()
    {
        _cam = Camera.main;
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapBounds = _tilemapRenderer.localBounds;
        _startingPos = transform.position;
        _length = _tilemapBounds.size.x;
    }

    void FixedUpdate()
    {
        // Calculate the distance the camera has moved since the start position
        float distanceX = (_cam.transform.position.x - _startingPos.x) * parallaxAmountX;
        float distanceY = (_cam.transform.position.y -  _startingPos.y) * parallaxAmountY;

        // Apply the parallaxeffect on both X and Y relative to the start position
        transform.position = new Vector2(_startingPos.x + distanceX, _startingPos.y + distanceY);

        // Infinite scrolling logic for the X axis
        float movementX = (_cam.transform.position.x -  _camStartingPosition.x) * (1 * parallaxAmountX);

        if (loop)
        {
            if (movementX > _startingPos.x + _length )
            {
                _startingPos.x += _length;
            }
            else if (movementX < _startingPos.x + _length)
            {
                _startingPos.x -= _length;
            }
        }
    }
}
