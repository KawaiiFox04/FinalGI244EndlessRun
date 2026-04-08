using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle3D : MonoBehaviour
{
    [Header("Despawn")]
    public float despawnX = -14f;

    [Header("Damage")]
    public int damageAmount = 1;

    public int PrefabIndex { get; private set; }

    private ObstacleSpawner spawner;
    private bool isStopped;
    private bool isInit;

    public void Init(int index, ObstacleSpawner spawnerRef)
    {
        PrefabIndex = index;
        spawner     = spawnerRef;
        isStopped   = false;
        isInit      = true;
    }

    public void Stop() => isStopped = true;

    void Update()
    {
        if (!isInit || isStopped) return;

        transform.Translate(
            Vector3.left * ObstacleSpawner.CurrentSpeed * Time.deltaTime,
            Space.World
        );

        if (transform.position.x < despawnX)
            ReturnSelf();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(damageAmount);
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