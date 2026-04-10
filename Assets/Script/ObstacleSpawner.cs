using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Position")]
    public float   spawnX          = 12f;
    public float spawnZ;
    public float[] spawnYPositions = { 0f };

    [Header("Base Speed")]
    public float initialSpeed      = 6f;
    public float maxSpeed          = 18f;
    public float speedIncreaseRate = 0.08f;

    [Header("Spawn Interval")]
    public float initialInterval = 5f;
    public float minInterval     = 1.5f;

    public static float CurrentSpeed    { get; private set; }
    public static float SpeedMultiplier { get; set; } = 1f;

    private float _baseSpeed;
    private float _spawnTimer;
    private float _currentInterval;
    private bool  _isRunning;

    private readonly Dictionary<int, Queue<GameObject>> _pool = new Dictionary<int, Queue<GameObject>>();
    private const int PoolSize = 6;

    private void Start()
    {
        SpeedMultiplier  = 1f;
        _baseSpeed       = initialSpeed;
        CurrentSpeed     = initialSpeed;
        InitPool();

        if (GameplayManager.Instance == null)
            StartGame();
    }

    private void InitPool()
    {
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            _pool[i] = new Queue<GameObject>();
            for (int j = 0; j < PoolSize; j++)
            {
                var go = Instantiate(obstaclePrefabs[i]);
                go.SetActive(false);
                _pool[i].Enqueue(go);
            }
        }
    }

    public void StartGame()
    {
        _isRunning       = true;
        SpeedMultiplier  = 1f;
        _baseSpeed       = initialSpeed;
        CurrentSpeed     = initialSpeed;
        _currentInterval = initialInterval;
        _spawnTimer      = initialInterval;
    }

    public void StopGame()
    {
        _isRunning = false;
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            obs.Stop();
    }

    public void ResetGame()
    {
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            ReturnToPool(obs.gameObject, obs.PrefabIndex);
        StartGame();
    }

    private void Update()
    {
        if (!_isRunning) return;

        _baseSpeed   = Mathf.Min(_baseSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        CurrentSpeed = _baseSpeed * SpeedMultiplier;

        float t = Mathf.InverseLerp(initialSpeed, maxSpeed, _baseSpeed);
        _currentInterval = Mathf.Lerp(initialInterval, minInterval, t);

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnObstacle();
            _spawnTimer = _currentInterval;
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: ไม่มี Prefab!");
            return;
        }

        int   idx    = Random.Range(0, obstaclePrefabs.Length);
        float spawnY = spawnYPositions[Random.Range(0, spawnYPositions.Length)];
        var   go     = GetFromPool(idx);

        go.transform.position = new Vector3(spawnX, spawnY, spawnZ);
        go.transform.rotation = Quaternion.identity;

        var obs = go.GetComponent<Obstacle3D>();
        if (obs != null)
        {
            obs.Init(idx, this);
        }
        else
        {
            Debug.LogError($"'{go.name}' ไม่มี Obstacle3D Script!");
        }
    }

    private GameObject GetFromPool(int idx)
    {
        if (_pool.ContainsKey(idx) && _pool[idx].Count > 0)
        {
            var go = _pool[idx].Dequeue();
            go.SetActive(true);
            return go;
        }
        return Instantiate(obstaclePrefabs[idx]);
    }

    public void ReturnToPool(GameObject go, int idx)
    {
        go.SetActive(false);
        if (_pool.ContainsKey(idx))
            _pool[idx].Enqueue(go);
        else
            Destroy(go);
    }
}