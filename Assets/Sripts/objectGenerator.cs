using UnityEngine;
using System.Collections.Generic;

public class objectGenerator : MonoBehaviour
{
    [System.Serializable]
    public class SpawnRule
    {
        public GameObject prefab;
        public int count = 3;
        public PlacementType placementType;
        [Tooltip("Minimum height percentage (0-1)")]
        [Range(0, 1)]
        public float minHeightPercent = 0f;
        [Tooltip("Maximum height percentage (0-1)")]
        [Range(0, 1)]
        public float maxHeightPercent = 1f;
        [Tooltip("Minimum slope angle required (degrees)")]
        public float minSlope = 0f;
        [Tooltip("Distance threshold for proximity checks")]
        public float proximityThreshold = 10f;
    }

    public enum PlacementType
    {
        Random,
        Low,
        High,
        Steep,
        Isolated,
        NearOtherObjects
    }

    [Header("Generation Settings")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private float spawnHeightMultiplier = 1f;
    
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("Maximum slope for player spawn in degrees")]
    [SerializeField] private float maxPlayerSpawnSlope = 30f;
    [Range(0, 1)]
    [SerializeField] private float minPlayerSpawnHeightPercent = 0.3f;
    [Range(0, 1)]
    [SerializeField] private float maxPlayerSpawnHeightPercent = 0.7f;
    
    [Header("Spawn Rules")]
    [SerializeField] private List<SpawnRule> spawnRules = new List<SpawnRule>();

    private float[,] heightMap;
    private float maxTerrainHeight;
    private Vector3 terrainScale;
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private GameObject spawnedPlayer;

    private void Start()
    {
        // Subscribe to an event or method that signals when the map is generated
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
            if (mapGenerator == null)
            {
                Debug.LogError("MapGenerator not found!");
                return;
            }
        }

        // Initial generation
        InitializeAndSpawn();
    }

    public void InitializeAndSpawn()
    {
        // Clear any existing spawned objects
        foreach (Vector3 pos in spawnedPositions)
        {
            GameObject obj = GetObjectAtPosition(pos);
            if (obj != null && obj != spawnedPlayer)
            {
                Destroy(obj);
            }
        }
        spawnedPositions.Clear();

        // Get the current heightmap from the mesh generator
        heightMap = Noise.GenerateNoiseMap(
            mapGenerator.mapWidth, 
            mapGenerator.mapHeight, 
            mapGenerator.seed,
            mapGenerator.noiseScale, 
            mapGenerator.octaves, 
            mapGenerator.persistance,
            mapGenerator.lacunarity, 
            mapGenerator.offset
        );

        // Calculate terrain scale
        terrainScale = new Vector3(
            mapGenerator.mapWidth,
            mapGenerator.meshHeightMultiplier,
            mapGenerator.mapHeight
        );

        // Find max height
        maxTerrainHeight = 0f;
        for (int y = 0; y < mapGenerator.mapHeight; y++)
        {
            for (int x = 0; x < mapGenerator.mapWidth; x++)
            {
                float height = heightMap[x, y] * mapGenerator.meshHeightMultiplier;
                if (height > maxTerrainHeight) maxTerrainHeight = height;
            }
        }

        // Spawn player first
        SpawnPlayer();

        // Then spawn other objects
        foreach (var rule in spawnRules)
        {
            SpawnObjects(rule);
        }
    }

    private GameObject GetObjectAtPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        return colliders.Length > 0 ? colliders[0].gameObject : null;
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is missing!");
            return;
        }

        Vector3? playerPosition = FindPlayerSpawnPosition();
        
        if (playerPosition.HasValue)
        {
            if (spawnedPlayer != null)
            {
                Destroy(spawnedPlayer);
            }

            spawnedPlayer = Instantiate(playerPrefab, playerPosition.Value, Quaternion.identity);
            
            // Add required components
            if (!spawnedPlayer.GetComponent<CharacterController>())
            {
                spawnedPlayer.AddComponent<CharacterController>();
            }
            
            playerController controller = spawnedPlayer.GetComponent<playerController>();
            if (!controller)
            {
                controller = spawnedPlayer.AddComponent<playerController>();
            }

            // Setup ground check
            Transform groundChecker = new GameObject("GroundChecker").transform;
            groundChecker.SetParent(spawnedPlayer.transform);
            groundChecker.localPosition = new Vector3(0, -1f, 0);
            
            controller.controller = spawnedPlayer.GetComponent<CharacterController>();
            controller.groundChecker = groundChecker;
            controller.groundMask = LayerMask.GetMask("Default");
            
            spawnedPositions.Add(playerPosition.Value);
        }
        else
        {
            Debug.LogError("Could not find suitable player spawn position!");
        }
    }

    private Vector3? FindPlayerSpawnPosition(int maxAttempts = 200)
    {
        float minHeight = maxTerrainHeight * minPlayerSpawnHeightPercent;
        float maxHeight = maxTerrainHeight * maxPlayerSpawnHeightPercent;
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 position = GetRandomTerrainPosition();
            float slope = CalculateSlope(position);
            
            if (slope <= maxPlayerSpawnSlope && 
                position.y >= minHeight && 
                position.y <= maxHeight && 
                !IsNearAnySpawnedObject(position, 5f))
            {
                position.y += 1f;
                return position;
            }
        }
        
        return null;
    }

    private Vector3 GetRandomTerrainPosition()
    {
        int x = Random.Range(0, mapGenerator.mapWidth);
        int z = Random.Range(0, mapGenerator.mapHeight);
        
        float height = heightMap[x, z] * mapGenerator.meshHeightMultiplier;
        
        return new Vector3(
            x - mapGenerator.mapWidth/2f,  // Center the position
            height,
            z - mapGenerator.mapHeight/2f
        );
    }

    private void SpawnObjects(SpawnRule rule)
    {
        if (rule.prefab == null)
        {
            Debug.LogWarning("Prefab is missing for a spawn rule!");
            return;
        }

        for (int i = 0; i < rule.count; i++)
        {
            Vector3? position = FindValidSpawnPosition(rule);
            
            if (position.HasValue)
            {
                GameObject spawnedObject = Instantiate(rule.prefab, position.Value, Quaternion.identity);
                spawnedObject.tag = "Object";
                spawnedPositions.Add(position.Value);
            }
            else
            {
                Debug.LogWarning($"Could not find valid spawn position for {rule.prefab.name} after maximum attempts");
            }
        }
    }

    private Vector3? FindValidSpawnPosition(SpawnRule rule, int maxAttempts = 100)
    {
        float minHeight = maxTerrainHeight * rule.minHeightPercent;
        float maxHeight = maxTerrainHeight * rule.maxHeightPercent;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 position = GetRandomTerrainPosition();
            
            if (IsValidPosition(position, rule, minHeight, maxHeight) && 
                !IsNearPlayer(position, 5f))
            {
                return position;
            }
        }
        
        return null;
    }

    private bool IsValidPosition(Vector3 position, SpawnRule rule, float minHeight, float maxHeight)
    {
        float height = position.y;
        float slope = CalculateSlope(position);

        switch (rule.placementType)
        {
            case PlacementType.Low:
                if (height > minHeight) return false;
                break;

            case PlacementType.High:
                if (height < maxHeight) return false;
                break;

            case PlacementType.Steep:
                if (slope < rule.minSlope) return false;
                break;

            case PlacementType.Isolated:
                if (IsNearAnySpawnedObject(position, rule.proximityThreshold)) return false;
                break;

            case PlacementType.NearOtherObjects:
                if (!IsNearAnySpawnedObject(position, rule.proximityThreshold)) return false;
                break;
        }

        return height >= minHeight && height <= maxHeight;
    }

    private bool IsNearPlayer(Vector3 position, float threshold)
    {
        if (spawnedPlayer == null) return false;
        return Vector3.Distance(position, spawnedPlayer.transform.position) < threshold;
    }

    private float CalculateSlope(Vector3 position)
    {
        // Convert world position to heightmap coordinates
        int x = Mathf.RoundToInt(position.x + mapGenerator.mapWidth/2f);
        int z = Mathf.RoundToInt(position.z + mapGenerator.mapHeight/2f);
        
        if (x <= 0 || x >= mapGenerator.mapWidth-1 || z <= 0 || z >= mapGenerator.mapHeight-1)
            return 0f;

        float heightL = heightMap[x-1, z] * mapGenerator.meshHeightMultiplier;
        float heightR = heightMap[x+1, z] * mapGenerator.meshHeightMultiplier;
        float heightU = heightMap[x, z+1] * mapGenerator.meshHeightMultiplier;
        float heightD = heightMap[x, z-1] * mapGenerator.meshHeightMultiplier;

        Vector3 normal = new Vector3(heightL - heightR, 2f, heightD - heightU).normalized;
        return Vector3.Angle(normal, Vector3.up);
    }

    private bool IsNearAnySpawnedObject(Vector3 position, float threshold)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < threshold)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 position in spawnedPositions)
        {
            Gizmos.DrawWireSphere(position, 1f);
        }
        
        if (spawnedPlayer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnedPlayer.transform.position, 1.5f);
        }
    }
}