// TrackSpawner.cs
using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public GameObject[] trackPrefabs;  // ใส่ TrackSegment Prefab
    public int initialTiles = 5;       // จำนวน Tile ตอนเริ่ม
    public float tileLength = 20f;     // ต้องตรงกับ Scale Z ของ Prefab

    private float nextSpawnZ = 0f;
    private List<GameObject> spawnedTiles = new List<GameObject>();
    private int maxTiles = 8;

    void Start()
    {
        for (int i = 0; i < initialTiles; i++)
            SpawnTile();
    }

    void Update()
    {
        // Spawn Tile ใหม่เมื่อ Player เข้าใกล้
        if (Camera.main.transform.position.z + 60f > nextSpawnZ)
            SpawnTile();

        // ลบ Tile เก่าที่ Player ผ่านไปแล้ว
        if (spawnedTiles.Count > maxTiles)
        {
            Destroy(spawnedTiles[0]);
            spawnedTiles.RemoveAt(0);
        }
    }

    void SpawnTile()
    {
        int rand = Random.Range(0, trackPrefabs.Length);
        GameObject tile = Instantiate(
            trackPrefabs[rand],
            new Vector3(0, 0, nextSpawnZ),
            Quaternion.identity
        );
        spawnedTiles.Add(tile);
        nextSpawnZ += tileLength;
    }
}