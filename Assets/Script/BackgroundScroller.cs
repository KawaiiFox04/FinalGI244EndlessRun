using UnityEngine;

// ติด Script นี้กับ GameObject ฉากหลังแต่ละชั้น
// ฉากหลังจะเลื่อนซ้ายตาม Speed ของ Obstacle และ Reset Position อัตโนมัติ
public class BackgroundScroller : MonoBehaviour
{
    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxFactor = 1f;
    // 1.0 = เร็วเท่า Obstacle (พื้น / ฉากใกล้)
    // 0.5 = ช้ากว่าครึ่ง   (ฉากกลาง)
    // 0.2 = ช้ามาก         (ฉากหลังไกล เช่น ท้องฟ้า)

    [Header("Loop Settings")]
    [Tooltip("ความกว้างของ Sprite/Mesh ในหน่วย World Unit\n" +
             "เมื่อเลื่อนไปเท่านี้จะ Reset กลับ")]
    public float tileWidth = 20f;

    // ตำแหน่งเริ่มต้น (จำไว้เพื่อ Reset)
    private float startX;
    private bool  isStopped;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        if (isStopped) return;

        float speed = ObstacleSpawner.CurrentSpeed * parallaxFactor;
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // ถ้าเลื่อนไปเกิน tileWidth → Reset กลับ (Seamless Loop)
        if (transform.position.x <= startX - tileWidth)
        {
            Vector3 pos = transform.position;
            pos.x += tileWidth;
            transform.position = pos;
        }
    }

    // ----------------------------------------------------------------
    //  Public API สำหรับ GameManager
    // ----------------------------------------------------------------
    public void Stop()          => isStopped = true;
    public void Resume()        => isStopped = false;

    public void ResetPosition()
    {
        transform.position = new Vector3(startX,
            transform.position.y,
            transform.position.z);
    }
}