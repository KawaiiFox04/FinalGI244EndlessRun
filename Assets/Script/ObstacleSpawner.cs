using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Position")]
    public float spawnX       = 12f;   // ระยะขวาจอ
    public float spawnZ       = 0f;    // ล็อค Z ให้ตรงกับ Player
    public float[] spawnYPositions = { 0f, 1.5f }; // ความสูงที่ Spawn ได้

    [Header("Speed")]
    public float initialSpeed      = 6f;
    public float maxSpeed          = 18f;
    public float speedIncreaseRate = 0.08f; // หน่วย/วินาที

    [Header("Spawn Interval")]
    public float initialInterval = 2f;
    public float minInterval     = 0.5f;

    // Static ให้ Script อื่นอ่านได้
    public static float CurrentSpeed { get; private set; }

    private float spawnTimer;
    private float currentInterval;
    private bool  isRunning;

    // Object Pool
    private Dictionary<int, Queue<GameObject>> pool = new();
    private const int PoolSizePerPrefab = 6;

    // ================================================================
    //  Init
    // ================================================================
    void Start()
    {
        CurrentSpeed = initialSpeed;
        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            pool[i] = new Queue<GameObject>();
            for (int j = 0; j < PoolSizePerPrefab; j++)
            {
                var go = Instantiate(obstaclePrefabs[i]);
                go.SetActive(false);
                pool[i].Enqueue(go);
            }
        }
    }

    // ================================================================
    //  Game State
    // ================================================================
    public void StartGame()
    {
        isRunning        = true;
        CurrentSpeed     = initialSpeed;
        currentInterval  = initialInterval;
        spawnTimer       = currentInterval;
    }

    public void StopGame()
    {
        isRunning = false;
        // หยุด Obstacle ที่กำลังวิ่งอยู่ทั้งหมด
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            obs.Stop();
    }

    public void ResetGame()
    {
        foreach (var obs in FindObjectsByType<Obstacle3D>(FindObjectsSortMode.None))
            ReturnToPool(obs.gameObject, obs.PrefabIndex);
        StartGame();
    }

    // ================================================================
    //  Update Loop
    // ================================================================
    void Update()
    {
        if (!isRunning) return;

        // เพิ่มความเร็ว
        CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);

        // คำนวณ Interval จาก t (0→1)
        float t = Mathf.InverseLerp(initialSpeed, maxSpeed, CurrentSpeed);
        currentInterval = Mathf.Lerp(initialInterval, minInterval, t);

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();
            spawnTimer = currentInterval + Random.Range(-0.15f, 0.15f);
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;

        int   idx    = Random.Range(0, obstaclePrefabs.Length);
        float spawnY = spawnYPositions[Random.Range(0, spawnYPositions.Length)];
        var   go     = GetFromPool(idx);

        go.transform.position = new Vector3(spawnX, spawnY, spawnZ);
        go.transform.rotation = Quaternion.identity;

        var obs = go.GetComponent<Obstacle3D>();
        obs?.Init(idx, this);
    }

    // ================================================================
    //  Pool Helpers
    // ================================================================
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
        if (pool.ContainsKey(idx))
            pool[idx].Enqueue(go);
        else
            Destroy(go);
    }
}