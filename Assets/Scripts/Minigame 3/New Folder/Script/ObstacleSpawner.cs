using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private Transform shipTransform;  // New reference for the ship
    [SerializeField] private float spawnRate = 5f;
    [SerializeField] private float obstacleSpeed = 5f;
    [SerializeField] private float spawnDistance = 20f;
    
    private float nextSpawnTime;

    void Start()
    {
        // Add null checks
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogError("No obstacle prefabs assigned to spawner!");
            enabled = false;
            return;
        }

        if (shipTransform == null)
        {
            Debug.LogError("Ship transform not assigned to spawner!");
            enabled = false;
            return;
        }

        nextSpawnTime = Time.time + spawnRate;
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || shipTransform == null) return;

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        if (obstaclePrefabs[randomIndex] == null)
        {
            Debug.LogError($"Obstacle prefab at index {randomIndex} is null!");
            return;
        }

        Vector3 spawnPosition = shipTransform.position + Vector3.forward * spawnDistance;
        spawnPosition.x = Random.Range(-2f, 2f);
        
        GameObject obstacle = Instantiate(obstaclePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        if (obstacle != null)
        {
            obstacle.AddComponent<ObstacleMovement>().Initialize(obstacleSpeed);
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
}
