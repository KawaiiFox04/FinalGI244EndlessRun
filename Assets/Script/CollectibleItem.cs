// CollectibleItem.cs
// ติดไว้ที่ Prefab ของ Item ทุกชนิด (Coin / Heal / SpeedBoost)
// Collider ต้องติ๊ก Is Trigger = true

using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Coin, Heal, SpeedBoost }

    [Header("Item Settings")]
    public ItemType itemType   = ItemType.Coin;
    public int      coinValue  = 1;    // ใช้เมื่อ ItemType = Coin
    public int      healValue  = 1;    // ใช้เมื่อ ItemType = Heal

    [Header("Movement")]
    public float despawnX = -14f;

    void Update()
    {
        // Item เคลื่อนที่ไปพร้อม Obstacle (ความเร็วเดียวกัน)
        transform.Translate(Vector3.left * ObstacleSpawner.CurrentSpeed * Time.deltaTime, Space.World);
        if (transform.position.x < despawnX) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        switch (itemType)
        {
            case ItemType.Coin:
                GameplayManager.Instance?.AddCoin(coinValue);
                break;

            case ItemType.Heal:
                pc.Heal(healValue);
                break;

            case ItemType.SpeedBoost:
                GameplayManager.Instance?.ActivateSpeedBoost();
                break;
        }

        Destroy(gameObject);
    }
}