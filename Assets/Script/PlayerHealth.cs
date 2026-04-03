using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 3;
    public int currentHp;
    public bool isShielded = false;
    public GameObject shieldVFX;

    void Start()
    {
        currentHp = maxHp;
    }

    public void ActivateShield(float duration)
    {
        isShielded = true;
        if (shieldVFX != null) shieldVFX.SetActive(true);
        if (HUDManager.instance != null) HUDManager.instance.ShowShield(true);
        Invoke(nameof(DeactivateShield), duration);
    }

    private void DeactivateShield()
    {
        isShielded = false;
        if (shieldVFX != null) shieldVFX.SetActive(false);
        if (HUDManager.instance != null) HUDManager.instance.ShowShield(false);
    }

    public void TakeDamage()
    {
        if (isShielded)
        {
            DeactivateShield();
            return;
        }

        currentHp--;

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}