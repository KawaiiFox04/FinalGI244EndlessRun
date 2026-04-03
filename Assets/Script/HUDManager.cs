using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [Header("UI References")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinText;
    public Image shieldIcon;
    public Image speedIcon;

    private float distance = 0f;
    private int coins = 0;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        distance += Time.deltaTime * GameManager.instance.currentSpeed;
        distanceText.text = Mathf.FloorToInt(distance) + " m";

        GameManager.instance.AddScore((int)(Time.deltaTime * 10));
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        coinText.text = "x" + coins;
        GameManager.instance.AddCoin(amount);
    }

    public void ShowShield(bool active)
    {
        if (shieldIcon != null)
            shieldIcon.gameObject.SetActive(active);
    }

    public void ShowSpeed(bool active)
    {
        if (speedIcon != null)
            speedIcon.gameObject.SetActive(active);
    }
}