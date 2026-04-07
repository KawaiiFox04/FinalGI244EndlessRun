using UnityEngine;

// ติด Script นี้ไว้ที่ Prefab สิ่งกีดขวางทุกชิ้น
// ใช้ Collider (3D) และ OnTriggerEnter (3D)
[RequireComponent(typeof(Collider))]
public class Obstacle3D : MonoBehaviour
{
    [Header("Despawn")]
    public float despawnX = -14f;

    // อ่านได้จาก ObstacleSpawner เพื่อ Return Pool
    public int PrefabIndex { get; private set; }

    private ObstacleSpawner spawner;
    private bool isStopped;
    private bool isInit;

    // ================================================================
    //  Init (เรียกจาก ObstacleSpawner)
    // ================================================================
    public void Init(int index, ObstacleSpawner spawnerRef)
    {
        PrefabIndex = index;
        spawner     = spawnerRef;
        isStopped   = false;
        isInit      = true;
    }

    public void Stop() => isStopped = true;

    // ================================================================
    //  Move
    // ================================================================
    void Update()
    {
        if (!isInit || isStopped) return;

        // เคลื่อนที่ไปทางซ้ายแกน X เท่านั้น (เหมือน 2D)
        transform.Translate(Vector3.left * ObstacleSpawner.CurrentSpeed * Time.deltaTime,
            Space.World);

        if (transform.position.x < despawnX)
            ReturnSelf();
    }

    // ================================================================
    //  Collision — ใช้ OnTriggerEnter (3D) ไม่ใช่ 2D
    // ================================================================
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.Die();
            ReturnSelf();
        }
    }

    void ReturnSelf()
    {
        isInit = false;
        if (spawner != null)
            spawner.ReturnToPool(gameObject, PrefabIndex);
        else
            gameObject.SetActive(false);
    }
}