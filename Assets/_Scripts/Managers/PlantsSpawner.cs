using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class PlantSpawnData
{
    public GameObject prefab;
    public float initialSun;
    public float initialWater;
    public int initialMusic;
    public int initialPoo;
}

public class PlantsSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<PlantSpawnData> plantsToSpawn;
    private int nextSpawnIndex = 0;
    public float spawnInterval = 10;
    public int maxPlants = 5;
    public float plantCheckRadius = 1f; // Radius to check for overlaps

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    private float timer = 0f;
    private List<GameObject> spawnedPlants = new List<GameObject>();

    private void Start()
    {
        timer = spawnInterval; // so that we immediately spawn the first one
    }

    void Update()
    {
        if (plantsToSpawn.Count == 0 || spawnArea == null) return;

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
                if (nextSpawnIndex >= plantsToSpawn.Count) { return; }// nothing left to spawn

                var data = plantsToSpawn[nextSpawnIndex];
                var prefab = data.prefab;
                Debug.Log("Next plant is: " + prefab.name);
                nextSpawnIndex++;

                GameObject newPlant = Instantiate(prefab, spawnPos, Quaternion.identity);
                var plantScript = newPlant.GetComponent<Plant>();
                plantScript.initialSun = data.initialSun;
                plantScript.initialWater = data.initialWater;
                plantScript.initialMusic = data.initialMusic;   
                plantScript.initialPoo = data.initialPoo;
                // TODO: init stats

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