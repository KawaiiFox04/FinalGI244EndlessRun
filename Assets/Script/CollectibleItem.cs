using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Coin, Heal, SpeedBoost }

    [Header("Item Settings")]
    public ItemType itemType  = ItemType.Coin;
    public int      coinValue = 1;
    public int      healValue = 1;

    [Header("Movement")]
    public float despawnX = -14f;

    private Transform _transform;

    private void Awake() => _transform = transform;

    private void Update()
    {
        float moveDist      = ObstacleSpawner.CurrentSpeed * Time.deltaTime;
        _transform.Translate(Vector3.left * moveDist, Space.World);

        if (_transform.position.x < despawnX)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        switch (itemType)
        {
            case ItemType.Coin:
                if (GameplayManager.Instance != null)
                    GameplayManager.Instance.AddCoin(coinValue);
                break;
            case ItemType.Heal:
                pc.Heal(healValue);
                break;
            case ItemType.SpeedBoost:
                if (GameplayManager.Instance != null)
                    GameplayManager.Instance.ActivateSpeedBoost();
                break;
        }

        Destroy(gameObject);
    }
}