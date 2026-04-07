using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController      player;
    public ObstacleSpawner       spawner;
    public BackgroundScroller[]  backgroundLayers;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject      gameOverPanel;
    public GameObject      startPanel;

    private float score;
    private float bestScore;
    private bool  isPlaying;

    // ================================================================
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance  = this;
        bestScore = PlayerPrefs.GetFloat("BestScore", 0f);
    }

    void Start() => ShowStart();

    void Update()
    {
        if (!isPlaying) return;
        score += Time.deltaTime * ObstacleSpawner.CurrentSpeed * 0.5f;
        scoreText?.SetText(Mathf.FloorToInt(score).ToString("N0"));
    }

    // ================================================================
    //  UI Flow
    // ================================================================
    void ShowStart()
    {
        startPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        isPlaying = false;
    }

    // เรียกจากปุ่ม Start ใน Canvas
    public void StartGame()
    {
        score     = 0f;
        isPlaying = true;

        startPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);

        player?.ResetPlayer();
        spawner?.StartGame();

        foreach (var bg in backgroundLayers)
        {
            bg?.ResetPosition();
            bg?.Resume();
        }
    }

    // ================================================================
    //  Called by PlayerController.Die()
    // ================================================================
    public void OnPlayerDied()
    {
        isPlaying = false;

        spawner?.StopGame();
        foreach (var bg in backgroundLayers) bg?.Stop();

        // บันทึก Best Score
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetFloat("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        // แสดง Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            var labels = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var lbl in labels)
            {
                if (lbl.name.Contains("Score"))
                    lbl.SetText($"Score: {Mathf.FloorToInt(score):N0}\nBest: {Mathf.FloorToInt(bestScore):N0}");
            }
        }
    }

    // ปุ่ม Retry
    public void RestartGame()
    {
        spawner?.ResetGame();
        StartGame();
    }
}