using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Position")]
    public float   spawnX          = 12f;
    public float   spawnZ          = 0f;
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

    private float baseSpeed;
    private float spawnTimer;
    private float currentInterval;
    private bool  isRunning;

    private Dictionary<int, Queue<GameObject>> pool = new();
    private const int PoolSize = 6;

    void Start()
    {
        SpeedMultiplier = 1f;
        baseSpeed       = initialSpeed;
        CurrentSpeed    = initialSpeed;
        InitPool();

        if (GameplayManager.Instance == null)
            StartGame();
    }

    void InitPool()
    {
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            pool[i] = new Queue<GameObject>();
            for (int j = 0; j < PoolSize; j++)
            {
                var go = Instantiate(obstaclePrefabs[i]);
                go.SetActive(false);
                pool[i].Enqueue(go);
            }
        }
    }

    public void StartGame()
    {
        isRunning       = true;
        SpeedMultiplier = 1f;
        baseSpeed       = initialSpeed;
        CurrentSpeed    = initialSpeed;
        currentInterval = initialInterval;
        spawnTimer      = initialInterval;
    }

    public void StopGame()
    {
        isRunning = false;
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            obs.Stop();
    }

    public void ResetGame()
    {
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            ReturnToPool(obs.gameObject, obs.PrefabIndex);
        StartGame();
    }

    void Update()
    {
        if (!isRunning) return;

        baseSpeed    = Mathf.Min(baseSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        CurrentSpeed = baseSpeed * SpeedMultiplier;

        float t = Mathf.InverseLerp(initialSpeed, maxSpeed, baseSpeed);
        currentInterval = Mathf.Lerp(initialInterval, minInterval, t);

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();
            spawnTimer = currentInterval;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: ไม่มี Prefab ใน Obstacle Prefabs!");
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
            Debug.Log($"Spawned at {go.transform.position}");
        }
        else
        {
            Debug.LogError($"'{go.name}' ไม่มี Obstacle3D Script!");
        }
    }

    GameObject GetFromPool(int idx)
    {
        if (pool.ContainsKey(idx) && pool[idx].Count > 0)
        {
            var go = pool[idx].Dequeue();
            go.SetActive(true);
            return go;
        }
        return Instantiate(obstaclePrefabs[idx]);
    }

    public void ReturnToPool(GameObject go, int idx)
    {
        go.SetActive(false);
        if (pool.ContainsKey(idx)) pool[idx].Enqueue(go);
        else Destroy(go);
    }
}