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
    private float spawnInterval = 10f; // Time between zombie spawns
    [SerializeField]
    private float zombieSpeed = 1.5f; // Speed at which zombies move
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
            Debug.Log("Spawning Zombie at entry point");
            SpawnZombie();
            CurrentZombiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
            spawnInterval-= 0.5f;
        }
    }
    private void SpawnZombie()
    {
        // Create a new zombie and set up their roaming routine
        int spawnPosition = UnityEngine.Random.Range(0, entryPoints.Length);
        Vector3Int spawnPoint = entryPoints[spawnPosition];

        GameObject zombie = Instantiate(zombiePrefab, grid.CellToWorld(spawnPoint), Quaternion.identity);
        //Customer customerEntity = customer.AddComponent<Customer>();


        //customerEntity.Setup(exitPoint, customerSpeed, shelfWaitTime, ref grid, ref placementSystem);
        //customerEntity.StartCustomer();
    }

    public void BeginZombieSpawner()
    {
        Debug.Log("Starting Customer Spawner");
        StartCoroutine(SpawnZombies());
    }
}