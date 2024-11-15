using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectGenerator : MonoBehaviour {
    public GameObject healthPotionPrefab;
    public GameObject coinPrefab;
    public GameObject weaponPrefab;
    public GameObject magicPickUpPrefab;
    public GameObject shieldPrefab;
    public GameObject trapPrefab;

    public int objectsPerType = 3;
    public Terrain terrain; // Reference to the terrain

    void Start()
    {
        SpawnObjects(healthPotionPrefab, objectsPerType, "low");
        SpawnObjects(coinPrefab, objectsPerType, "random");
        SpawnObjects(weaponPrefab, objectsPerType, "steep");
        SpawnObjects(magicPickUpPrefab, objectsPerType, "isolated");
        SpawnObjects(shieldPrefab, objectsPerType, "high");
        SpawnObjects(trapPrefab, objectsPerType, "nearOtherObjects");
    }

    void SpawnObjects(GameObject objectPrefab, int count, string rule)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = DetermineSpawnPosition(rule);
            Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 DetermineSpawnPosition(string rule)
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;
        
        float xPos = Random.Range(0, terrainWidth);
        float zPos = Random.Range(0, terrainHeight);
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));

        switch (rule)
        {
            case "low":
                while (yPos > 10) { // Adjust as needed for "low" terrain
                    xPos = Random.Range(0, terrainWidth);
                    zPos = Random.Range(0, terrainHeight);
                    yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
                }
                break;
            case "high":
                while (yPos < 20) { // Adjust for higher terrain levels
                    xPos = Random.Range(0, terrainWidth);
                    zPos = Random.Range(0, terrainHeight);
                    yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
                }
                break;
            case "steep":
                // Find a steep location (requires additional checks for slope)
                // Optional: calculate slope and find steep areas
                break;
            case "random":
                // No special conditions for random placement
                break;
            case "isolated":
                // Define an area farther from other objects for isolation
                break;
            case "nearOtherObjects":
                // Place trap near a certain radius of other objects
                break;
        }

        return new Vector3(xPos, yPos, zPos);
    }
}
