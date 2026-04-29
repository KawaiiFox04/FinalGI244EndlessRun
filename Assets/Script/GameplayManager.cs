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
    public TextMeshProUGUI statusText;

    [Header("Score")]
    public float scoreMultiplier = 0.5f;

    [Header("Heal Item")]
    public GameObject healItemPrefab;
    public float      healSpawnInterval  = 30f;  
    public float      healSpawnX         = 12f;
    public float[]    itemSpawnYPositions = { 0f, 1.5f };

    [Header("Coin Item")]
    public GameObject coinItemPrefab;
    public float      coinSpawnInterval = 3f;

    [Header("SpeedBoost Item")]
    public GameObject speedBoostItemPrefab;
    public float      speedBoostSpawnInterval = 15f;  
    public float      speedBoostDuration      = 5f;

    [Header("Camera Flip")]
    public float flipInterval = 15f;
    public float flipDuration = 8f;

    [Header("Size Change")]
    public float sizeChangeInterval = 5f;
    public float smallDuration      = 4f;

    private float _score;
    private int   _coins;
    private bool  _isPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        StartGame();
    }

    private void Update()
    {
        if (!_isPlaying) return;
        _score += Time.deltaTime * ObstacleSpawner.CurrentSpeed * scoreMultiplier;
        if (scoreText != null) scoreText.SetText(Mathf.FloorToInt(_score).ToString("N0"));
    }

    private void StartGame()
    {
        _score    = 0f;
        _coins    = 0;
        _isPlaying = true;

        RefreshCoinUI();
        if (statusText != null) statusText.SetText("");
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameplayBGM();

        if (player != null) player.ResetPlayer();
        if (spawner != null) spawner.StartGame();

        foreach (var bg in backgroundLayers)
        {
            if (bg == null) continue;
            bg.ResetPosition();
            bg.Resume();
        }

        StartCoroutine(SpawnCoinRoutine());
        StartCoroutine(SpawnHealRoutine());
        StartCoroutine(SpawnSpeedBoostRoutine());
        StartCoroutine(FlipCameraRoutine());
        StartCoroutine(SizeChangeRoutine());
    }

    public void AddCoin(int amount = 1)
    {
        _coins += amount;
        RefreshCoinUI();
    }

    private void RefreshCoinUI()
    {
        if (coinText != null) coinText.SetText($"Coin: {_coins}");
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        if (player != null) player.isInvincible = true;
        ObstacleSpawner.SpeedMultiplier = 2f;
        foreach (var bg in backgroundLayers)
            if (bg != null) bg.SetSpeedMultiplier(2f);

        ShowStatus("SPEED BOOST!");
        yield return new WaitForSeconds(speedBoostDuration);

        if (player != null) player.isInvincible = false;
        ObstacleSpawner.SpeedMultiplier = 1f;
        foreach (var bg in backgroundLayers)
            if (bg != null) bg.SetSpeedMultiplier(1f);

        ShowStatus("");
    }

    private IEnumerator SpawnCoinRoutine()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(coinSpawnInterval);
            if (!_isPlaying) break;
            SpawnItem(coinItemPrefab);
        }
    }

    private IEnumerator SpawnHealRoutine()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(healSpawnInterval);
            if (!_isPlaying) break;
            SpawnItem(healItemPrefab);
        }
    }

    private IEnumerator SpawnSpeedBoostRoutine()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(speedBoostSpawnInterval);
            if (!_isPlaying) break;
            SpawnItem(speedBoostItemPrefab);
        }
    }

    private void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;
        float y      = itemSpawnYPositions[Random.Range(0, itemSpawnYPositions.Length)];
        float spawnZ = player != null ? player.transform.position.z : 0f;
        Instantiate(prefab, new Vector3(healSpawnX, y, spawnZ), Quaternion.identity);
    }

    private IEnumerator FlipCameraRoutine()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(flipInterval);
            if (!_isPlaying) break;

            if (mainCamera != null)
                mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            ShowStatus("FLIP!");

            yield return new WaitForSeconds(flipDuration);

            if (mainCamera != null)
                mainCamera.transform.rotation = Quaternion.identity;
            ShowStatus("");
        }
    }

    private IEnumerator SizeChangeRoutine()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(sizeChangeInterval);
            if (!_isPlaying) break;
            
            if (player != null) player.SetSmallSize(true);
            
            SetWorldObjectsScale(0.5f);

            ShowStatus("SMALL!");
            yield return new WaitForSeconds(smallDuration);
            
            if (player != null) player.SetSmallSize(false);
            SetWorldObjectsScale(1f);

            ShowStatus("");
        }
    }

    private void SetWorldObjectsScale(float scale)
    {
        var obstacles = FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None);
        foreach (var obs in obstacles)
            obs.transform.localScale = Vector3.one * scale;
        
        var items = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);
        foreach (var item in items)
            item.transform.localScale = Vector3.one * scale;
    }

    private void ShowStatus(string msg)
    {
        if (statusText != null) statusText.SetText(msg);
    }

    public void OnPlayerDied()
    {
        if (!_isPlaying) return;
        _isPlaying = false;

        StopAllCoroutines();
        if (spawner != null) spawner.StopGame();
        foreach (var bg in backgroundLayers)
            if (bg != null) bg.Stop();

        if (mainCamera != null)
            mainCamera.transform.rotation = Quaternion.identity;

        GameData.CurrentScore = Mathf.FloorToInt(_score);
        GameData.CurrentCoins = _coins;
        GameData.TotalCoins  += _coins;

        if (GameData.CurrentScore > GameData.BestScore)
            GameData.BestScore = GameData.CurrentScore;

        GameData.SaveRecord(new RunRecord(GameData.CurrentScore, GameData.CurrentCoins));

        StartCoroutine(LoadGameOverAfterDelay(1.5f));
    }

    private IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
        SceneManager.LoadScene(gameOverSceneName);
    }
}