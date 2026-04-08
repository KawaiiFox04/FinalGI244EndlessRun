// BackgroundScroller.cs
// ติดไว้ที่ Background แต่ละชั้น

using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxFactor = 1f;

    [Header("Loop")]
    public float tileWidth = 20f;

    private float startX;
    private bool  isStopped;
    private float speedMultiplier = 1f;

    void Start() => startX = transform.position.x;

    void Update()
    {
        if (isStopped) return;
        float speed = ObstacleSpawner.CurrentSpeed * parallaxFactor * speedMultiplier;
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        if (transform.position.x <= startX - tileWidth)
        {
            var pos = transform.position;
            pos.x += tileWidth;
            transform.position = pos;
        }
    }

    public void Stop()                         => isStopped = true;
    public void Resume()                       => isStopped = false;
    public void SetSpeedMultiplier(float m)    => speedMultiplier = m;

    public void ResetPosition()
    {
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
    }
}