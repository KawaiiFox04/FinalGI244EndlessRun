using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI coinText;
    public GameObject newRecordBadge;

    void Start()
    {
        int score = GameManager.instance.currentScore;
        int coins = GameManager.instance.currentCoins;
        int best = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = "Score: " + score;
        coinText.text = "Coins: " + coins;

        if (score > best)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            best = score;
            newRecordBadge.SetActive(true);
        }
        else
        {
            newRecordBadge.SetActive(false);
        }

        bestText.text = "Best: " + best;

        HistoryManager.SaveRun(score, coins);
    }

    public void OnRestartButton()
    {
        GameManager.instance.ResetSession();
        SceneManager.LoadScene("GamePlay");
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("StartScreen");
    }
}