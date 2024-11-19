using System.Collections.Generic;
using UnityEngine;

public class objectGenerator : MonoBehaviour
{
    public GameObject healthPotionPrefab;
    public GameObject coinPrefab;
    public GameObject weaponPrefab;
    public GameObject bushPrefab;
    public GameObject rockPrefab;
    public GameObject treePrefab;
    public GameObject playerPrefab;

    public int objectsPerType = 3; // At least 3 objects per type
    public ProceduralTerrainGenerator terrainGenerator;

    private List<Vector3> existingObjects = new List<Vector3>();
    private float isolationRadius = 15f;
    private float proximityRadius = 10f;

    private float[,] heightMap;
    private Vector3 terrainScale;

    void Start()
    {
        if (terrainGenerator == null)
        {
            Debug.LogError("ProceduralTerrainGenerator not assigned!");
            return;
        }

        terrainGenerator.OnTerrainGenerated += OnTerrainGenerated;
    }

    private void OnTerrainGenerated()
    {
        Debug.Log("Terrain generated. Starting object placement...");
        heightMap = terrainGenerator.heightMap;
        terrainScale = terrainGenerator.transform.localScale;

        // Spawn player and objects
        SpawnPlayer();
        SpawnObjects(healthPotionPrefab, objectsPerType, "low");
        SpawnObjects(coinPrefab, objectsPerType, "random");
        SpawnObjects(weaponPrefab, objectsPerType, "steep");
        SpawnObjects(bushPrefab, objectsPerType, "isolated");
        SpawnObjects(rockPrefab, objectsPerType, "high");
        SpawnObjects(treePrefab, objectsPerType, "nearOtherObjects");
    }

    void SpawnPlayer()
    {
        Vector3 spawnPosition = DetermineSpawnPosition("random");
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    void SpawnObjects(GameObject objectPrefab, int count, string rule)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = DetermineSpawnPosition(rule);
            Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

            if (rule == "isolated" || rule == "nearOtherObjects")
            {
                existingObjects.Add(spawnPosition);
            }
        }
    }

    Vector3 DetermineSpawnPosition(string rule)
    {
        Vector3 spawnPosition;
        do
        {
            int x = Random.Range(0, heightMap.GetLength(0));
            int z = Random.Range(0, heightMap.GetLength(1));
            float normalizedHeight = heightMap[x, z];

            float y = terrainGenerator.heightCurve.Evaluate(normalizedHeight) * terrainGenerator.maxHeight;
            spawnPosition = new Vector3(x * terrainScale.x, y, z * terrainScale.z);
        }
        while (!ValidateSpawnPosition(spawnPosition, rule));

        return spawnPosition;
    }

    bool ValidateSpawnPosition(Vector3 position, string rule)
    {
        float y = position.y;

        switch (rule)
        {
            case "low": return y < terrainGenerator.maxHeight * 0.3f;
            case "high": return y > terrainGenerator.maxHeight * 0.7f;
            case "steep": return IsSteep(position);
            case "isolated": return IsFarFromExistingObjects(position);
            case "nearOtherObjects": return IsCloseToExistingObjects(position);
            default: return true;
        }
    }

    bool IsSteep(Vector3 position)
    {
        // Replace placeholder logic with terrain gradient calculation
        return Random.value > 0.5f;
    }

    bool IsFarFromExistingObjects(Vector3 position)
    {
        foreach (Vector3 obj in existingObjects)
        {
            if (Vector3.Distance(new Vector3(position.x, 0, position.z), obj) < isolationRadius)
                return false;
        }
        return true;
    }

    bool IsCloseToExistingObjects(Vector3 position)
    {
        foreach (Vector3 obj in existingObjects)
        {
            if (Vector3.Distance(new Vector3(position.x, 0, position.z), obj) < proximityRadius)
                return true;
        }
        return false;
    }
}

