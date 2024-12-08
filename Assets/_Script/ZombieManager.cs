using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    public GameObject zombiePrefab;
    private Vector3Int[] entryPoints = new Vector3Int[]
    {
        new Vector3Int(0, 0, 5),
        new Vector3Int(1, 0, 5),
        new Vector3Int(2, 0, 5),
        new Vector3Int(3, 0, 5),
        new Vector3Int(-1, 0, 5),
        new Vector3Int(-2, 0, 5),
        new Vector3Int(-3, 0, 5),        
    };

    public List<Vector3Int> shelves; // List of shelf locations
    private float spawnInterval = 5f; // Time between zombie spawns
    private float zombieSpeed = 1.5f; // Speed at which zombies move
    public float shelfWaitTime = 3f; // Time customers wait at each shelf
    private Vector3Int exitPoint = new Vector3Int(0, 0, -5);

    [SerializeField]
    Grid grid;
    [SerializeField]
    private PlacementSystem placementSystem;
    private int MaxZombiesToBeSpawned = 30;
    private int CurrentZombiesSpawned = 0;
    private IEnumerator SpawnZombies()
    {
        while (CurrentZombiesSpawned < MaxZombiesToBeSpawned)
        {
            Debug.Log("Spawning Zombie at entry point.");
            SpawnZombie();
            CurrentZombiesSpawned++;

            // Reduce spawn interval, with a minimum of 1 second
            spawnInterval = Mathf.Max(spawnInterval - 0.1f, 1f);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnZombie()
    {
        int spawnIndex = Random.Range(0, entryPoints.Length);
        Vector3Int spawnPoint = entryPoints[spawnIndex];

        // Create a new customer and set up their shopping routine
        GameObject zombie = Instantiate(zombiePrefab, grid.CellToWorld(spawnPoint), Quaternion.identity);
        //GameObject zombie = Instantiate(zombiePrefab, grid.CellToWorld(spawnPoint) + new Vector3(-0.5f, 0, -0.5f), Quaternion.identity);

        Zombie zombieEntity = zombie.AddComponent<Zombie>();

        zombieEntity.Setup(exitPoint, zombieSpeed, shelfWaitTime, ref grid, ref placementSystem);
        //customerEntity.StartCustomer();
    }

    public void BeginZombieSpawner()
    {
        Debug.Log("Starting Customer Spawner");
        StartCoroutine(SpawnZombies());
    }
}