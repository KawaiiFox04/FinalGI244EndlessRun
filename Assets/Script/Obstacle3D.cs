using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle3D : MonoBehaviour
{
    [Header("Despawn")]
    public float despawnX = -15f;

    [Header("Damage")]
    public int damageAmount = 1;

    public int PrefabIndex { get; private set; }

    private ObstacleSpawner _spawner;
    private bool _isStopped;
    private bool _isInit;

    public void Init(int index, ObstacleSpawner spawnerRef)
    {
        PrefabIndex = index;
        _spawner    = spawnerRef;
        _isStopped  = false;
        _isInit     = true;
    }

    public void Stop() => _isStopped = true;

    private void Update()
    {
        if (!_isInit || _isStopped) return;

        transform.Translate(
            Vector3.left * (ObstacleSpawner.CurrentSpeed * Time.deltaTime),
            Space.World
        );

        if (transform.position.x < despawnX)
            ReturnSelf();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(damageAmount);
            ReturnSelf();
        }
    }

    private void ReturnSelf()
    {
        _isInit = false;
        if (_spawner != null)
            _spawner.ReturnToPool(gameObject, PrefabIndex);
        else
            gameObject.SetActive(false);
    }
}