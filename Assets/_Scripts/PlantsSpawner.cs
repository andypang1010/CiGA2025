using UnityEngine;
using System.Collections.Generic;


public class PlantsSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] plantPrefabs;
    public float spawnInterval = 5f;
    public int maxPlants = 10;
    public float plantCheckRadius = 0.5f; // Radius to check for overlaps

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    private float timer = 0f;
    private List<GameObject> spawnedPlants = new List<GameObject>();

    void Update()
    {
        if (plantPrefabs.Length == 0 || spawnArea == null) return;

        // Clean up destroyed plants from the list
        spawnedPlants.RemoveAll(p => p == null);

        if (spawnedPlants.Count >= maxPlants) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnPlant();
            timer = 0f;
        }
    }

    private void TrySpawnPlant()
{
    for (int attempts = 0; attempts < 10; attempts++)
    {
        Vector3 spawnPos = GetRandomPositionInArea();

        // Check for other plant colliders nearby
        Collider[] hits = Physics.OverlapSphere(spawnPos, plantCheckRadius);
        bool spotIsClear = true;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Plant"))
            {
                spotIsClear = false;
                break;
            }
        }

        if (spotIsClear)
        {
            GameObject prefab = plantPrefabs[Random.Range(0, plantPrefabs.Length)];
            GameObject newPlant = Instantiate(prefab, spawnPos, Quaternion.identity);
            newPlant.tag = "Plant"; // Just in case the prefab isn't tagged already
            spawnedPlants.Add(newPlant);
           // Debug.Log("🌱 Spawned plant at " + spawnPos);
            return;
        }
    }

    Debug.LogWarning("Could not find a clear spot to spawn after 10 attempts.");
}

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 center = spawnArea.bounds.center;
        Vector3 size = spawnArea.bounds.size;

        return new Vector3(
            Random.Range(center.x - size.x / 2, center.x + size.x / 2),
            center.y,
            Random.Range(center.z - size.z / 2, center.z + size.z / 2)
        );
    }
}