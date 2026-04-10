using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Speed")]
    [Range(0f, 1f)]
    public float parallaxFactor = 1f;

    [Header("Loop")]
    public float tileWidth = 20f;

    private float     _startX;
    private bool      _isStopped;
    private float     _speedMultiplier = 1f;
    private Transform _transform;

    private void Awake() => _transform = transform;

    private void Start() => _startX = _transform.position.x;

    private void Update()
    {
        if (_isStopped) return;

        float moveDist = ObstacleSpawner.CurrentSpeed * parallaxFactor * _speedMultiplier * Time.deltaTime;
        _transform.Translate(Vector3.left * moveDist, Space.World);

        Vector3 pos = _transform.position;
        if (pos.x <= _startX - tileWidth)
        {
            pos.x              += tileWidth;
            _transform.position = pos;
        }
    }

    public void Stop()                      => _isStopped = true;
    public void Resume()                    => _isStopped = false;
    public void SetSpeedMultiplier(float m) => _speedMultiplier = m;

    public void ResetPosition()
    {
        Vector3 pos         = _transform.position;
        pos.x               = _startX;
        _transform.position = pos;
    }
}