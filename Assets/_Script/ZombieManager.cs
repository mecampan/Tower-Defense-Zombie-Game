using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    public GameObject zombiePrefab;
    private Vector3Int[] entryPoints = new Vector3Int[]
    {
        //Left Side Spawns
        new Vector3Int(-5, 0, 2),
        new Vector3Int(-5, 0, -1),
        new Vector3Int(-5, 0, -4),
        // Upper Side Spawns
        new Vector3Int(-4, 0, 4),
        new Vector3Int(-1, 0, 4),
        new Vector3Int(3, 0, 4),
        // Right Side Spawns
        new Vector3Int(4, 0, 2),        
        new Vector3Int(4, 0, -1),        
        new Vector3Int(4, 0, -4),        
    };

    public List<Vector3Int> shelves; // List of shelf locations
    private float spawnInterval = 4f; // Time between zombie spawns
    private float zombieSpeed = 2f; // Speed at which zombies move
    [SerializeField]
    public float ZombieDeltaTime = 0.01f; // Time customers wait at each shelf
    private Vector3Int exitPoint = new Vector3Int(0, 0, -5);

    [SerializeField]
    Grid grid;
    [SerializeField]
    private PlacementSystem placementSystem;
    private int MaxZombiesToBeSpawned = 50;
    private int CurrentZombiesSpawned = 0;
    private IEnumerator SpawnZombies()
    {
        while (CurrentZombiesSpawned < MaxZombiesToBeSpawned)
        {
            SpawnZombie();
            CurrentZombiesSpawned++;

            yield return new WaitForSeconds(spawnInterval);
            spawnInterval-= 0.5f;
            if(spawnInterval < 1f)
            {
                spawnInterval = 1f;
            }
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

        zombieEntity.Setup(spawnPoint, exitPoint, zombieSpeed, ZombieDeltaTime, ref grid, ref placementSystem);
        //customerEntity.StartCustomer();
    }

    public void BeginZombieSpawner()
    {
        StartCoroutine(SpawnZombies());
    }
}