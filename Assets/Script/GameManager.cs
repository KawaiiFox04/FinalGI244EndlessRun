using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Session Data")]
    public int currentScore = 0;
    public int currentCoins = 0;
    public float currentSpeed = 10f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetSession()
    {
        currentScore = 0;
        currentCoins = 0;
        currentSpeed = 10f;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public void AddCoin(int amount)
    {
        currentCoins += amount;
    }
}