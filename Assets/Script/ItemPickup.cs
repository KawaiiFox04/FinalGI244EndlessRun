using System.Collections;
using UnityEngine;

public enum ItemType { SpeedBoost, Shield }

public class ItemPickup : MonoBehaviour
{
    public ItemType type;
    public float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        switch (type)
        {
            case ItemType.SpeedBoost:
                if (pm != null)
                    StartCoroutine(SpeedBoost(pm));
                if (HUDManager.instance != null)
                    HUDManager.instance.ShowSpeed(true);
                break;

            case ItemType.Shield:
                if (ph != null)
                    ph.ActivateShield(duration);
                break;
        }

        Destroy(gameObject);
    }

    private IEnumerator SpeedBoost(PlayerMovement pm)
    {
        pm.speed *= 1.5f;
        yield return new WaitForSeconds(duration);
        pm.speed /= 1.5f;
        if (HUDManager.instance != null)
            HUDManager.instance.ShowSpeed(false);
    }
}