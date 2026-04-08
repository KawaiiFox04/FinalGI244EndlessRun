// GameplayManager.cs
// ติดไว้ที่ GameObject ใน Scene: Gameplay
// ควบคุม Score, Coin, SpeedBoost, Flip Camera, Size Change

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Scene Names")]
    public string gameOverSceneName = "GameOver";

    [Header("References")]
    public PlayerController     player;
    public ObstacleSpawner      spawner;
    public BackgroundScroller[] backgroundLayers;
    public Camera               mainCamera;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI statusText;   // แสดงข้อความ "SPEED BOOST!" / "FLIP!" / "SMALL!"

    [Header("Score")]
    public float scoreMultiplier = 0.5f;

    [Header("Heal Item Spawn")]
    public GameObject healItemPrefab;
    public float      healSpawnInterval  = 5f;
    public float      healSpawnX         = 12f;
    public float[]    itemSpawnYPositions = { 0f, 1.5f };

    [Header("SpeedBoost Item Spawn")]
    public GameObject speedBoostItemPrefab;
    public float      speedBoostSpawnInterval = 15f;  // สุ่ม Spawn ทุก 15 วินาที
    public float      speedBoostDuration      = 5f;

    [Header("Camera Flip")]
    public float flipInterval = 15f;    // กลับหัวทุก 15 วินาที
    public float flipDuration = 8f;     // อยู่ในสถานะกลับหัวนาน 8 วินาที

    [Header("Size Change")]
    public float sizeChangeInterval = 5f;
    public float smallDuration      = 4f;

    // ---- Runtime ----
    private float score;
    private int   coins;
    private bool  isPlaying;
    private bool  isCameraFlipped;

    // ================================================================
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        StartGame();
    }

    void Update()
    {
        if (!isPlaying) return;
        score += Time.deltaTime * ObstacleSpawner.CurrentSpeed * scoreMultiplier;
        scoreText?.SetText(Mathf.FloorToInt(score).ToString("N0"));
    }

    // ================================================================
    //  Start
    // ================================================================
    void StartGame()
    {
        score     = 0;
        coins     = 0;
        isPlaying = true;

        RefreshCoinUI();
        statusText?.SetText("");

        player?.ResetPlayer();
        spawner?.StartGame();
        foreach (var bg in backgroundLayers) { bg?.ResetPosition(); bg?.Resume(); }

        // เริ่ม Coroutines ทั้งหมด
        StartCoroutine(SpawnHealRoutine());
        StartCoroutine(SpawnSpeedBoostRoutine());
        StartCoroutine(FlipCameraRoutine());
        StartCoroutine(SizeChangeRoutine());
    }

    // ================================================================
    //  Coin
    // ================================================================
    public void AddCoin(int amount = 1)
    {
        coins += amount;
        RefreshCoinUI();
    }

    void RefreshCoinUI() => coinText?.SetText($"🪙 {coins}");

    // ================================================================
    //  SpeedBoost
    // ================================================================
    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    IEnumerator SpeedBoostRoutine()
    {
        player.IsInvincible = true;
        ObstacleSpawner.SpeedMultiplier = 2f;
        foreach (var bg in backgroundLayers) bg?.SetSpeedMultiplier(2f);

        ShowStatus("⚡ SPEED BOOST!");
        yield return new WaitForSeconds(speedBoostDuration);

        player.IsInvincible = false;
        ObstacleSpawner.SpeedMultiplier = 1f;
        foreach (var bg in backgroundLayers) bg?.SetSpeedMultiplier(1f);

        ShowStatus("");
    }

    // ================================================================
    //  Spawn Routines
    // ================================================================
    IEnumerator SpawnHealRoutine()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(healSpawnInterval);
            if (!isPlaying) break;
            SpawnItem(healItemPrefab);
        }
    }

    IEnumerator SpawnSpeedBoostRoutine()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(speedBoostSpawnInterval);
            if (!isPlaying) break;
            SpawnItem(speedBoostItemPrefab);
        }
    }

    void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;
        float y   = itemSpawnYPositions[Random.Range(0, itemSpawnYPositions.Length)];
        float spawnZ = player != null ? player.transform.position.z : 0f;
        Instantiate(prefab, new Vector3(healSpawnX, y, spawnZ), Quaternion.identity);
    }

    // ================================================================
    //  Camera Flip (Upside Down ทุก 15 วินาที)
    // ================================================================
    IEnumerator FlipCameraRoutine()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(flipInterval);
            if (!isPlaying) break;

            isCameraFlipped = true;
            mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            ShowStatus("🙃 FLIP!");

            yield return new WaitForSeconds(flipDuration);

            isCameraFlipped = false;
            mainCamera.transform.rotation = Quaternion.identity;
            ShowStatus("");
        }
    }

    // ================================================================
    //  Size Change (ตัวเล็กลงทุก 5 วินาที)
    // ================================================================
    IEnumerator SizeChangeRoutine()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(sizeChangeInterval);
            if (!isPlaying) break;

            player?.SetSmallSize(true);
            ShowStatus("🐾 SMALL!");

            yield return new WaitForSeconds(smallDuration);

            player?.SetSmallSize(false);
            ShowStatus("");
        }
    }

    // ================================================================
    //  Status Text Helper
    // ================================================================
    void ShowStatus(string msg)
    {
        if (statusText != null) statusText.SetText(msg);
    }

    // ================================================================
    //  Game Over
    // ================================================================
    public void OnPlayerDied()
    {
        if (!isPlaying) return;
        isPlaying = false;

        StopAllCoroutines();
        spawner?.StopGame();
        foreach (var bg in backgroundLayers) bg?.Stop();

        // Reset กล้องถ้ากลับหัวอยู่
        mainCamera.transform.rotation = Quaternion.identity;

        // บันทึกข้อมูล
        GameData.CurrentScore = Mathf.FloorToInt(score);
        GameData.CurrentCoins = coins;
        GameData.TotalCoins  += coins;

        if (GameData.CurrentScore > GameData.BestScore)
            GameData.BestScore = GameData.CurrentScore;

        GameData.SaveRecord(new RunRecord(GameData.CurrentScore, GameData.CurrentCoins));

        // หน่วงเล็กน้อยก่อนเปลี่ยน Scene
        StartCoroutine(LoadGameOverAfterDelay(1.5f));
    }

    IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(gameOverSceneName);
    }
}