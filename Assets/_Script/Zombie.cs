using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Zombie : Entity
{
    [SerializeField]
    [Header("Zombie Testing")]
    private List<Vector3Int> shoppingList = new List<Vector3Int>();
    [SerializeField]
    [Header("Zombie Testing")]
    private Vector3Int exitPoint;
    private int currentTargetIndex = 0;
    private int health = 5;

    public void StartZombie()
    {
        StartCoroutine(StartRoaming());
    }
    void OnCollisionEnter(Collision other)
    {
        // Check if the colliding object is tagged as "Bullet"
        if (other.gameObject.CompareTag("Bullet"))
        {
            health -= 1;
            Debug.Log($"Zombie took {1} damage. Health remaining: {health}");

            if (health <= 0)
            {
                Destroy(gameObject); // Destroy the zombie
        }

        }
    }
    
    public void Setup(Vector3Int exit, float speed, float wait, ref Grid InputGrid, ref PlacementSystem inputPlacementSystem)
    {
        grid = InputGrid;
        placementSystem = inputPlacementSystem;

        
        GridData floorData = placementSystem.floorData;
        Dictionary<Vector3Int, PlacementData> placedObjects = floorData.GetAllPlacedObjects();


        Dictionary<int, List<Vector3Int>> storedTiles = new Dictionary<int, List<Vector3Int>>();
        foreach (var entry in placedObjects)
        {
            Vector3Int position = entry.Key;
            PlacementData data = entry.Value;

            HashSet<int> ids = data.IDs;

            foreach(int id in ids){
                if(storedTiles.ContainsKey(id)){
                    List<Vector3Int> outList;
                    storedTiles.TryGetValue(id, out outList);
                    if(outList != null){
                        outList.Add(position);
                    }
                    else{
                        outList = new List<Vector3Int>() {position};
                    }
                }
                else {
                    storedTiles.Add(id, new List<Vector3Int>() {position});
                }
            }

            //string ids = string.Join(", ", data.IDs); // Convert IDs to a comma-separated string

            //Debug.Log($"Position: {position}, IDs: [{ids}], PlacedObjectIndex: {data.PlacedObjectIndex}");
        }

        exitPoint = exit;
        moveSpeed = speed;
        waitTime = wait;

        // Start moving to random locations
        StartCoroutine(StartRoaming());
    }
    
    private IEnumerator StartRoaming()
    {
        while (true) // Keep the zombie moving
        {
            if (path == null || path.Count == 0)
            {
                Debug.Log("Finding path to the exit.");
                FindPath(exitPoint); // Compute a path tso the exit
            }

            if (path != null && path.Count > 0)
            {
                yield return new WaitForSeconds(2f); // Wait before the next step
                NAVIGATIONSTATUS navStatus = NavigatePath();
                Debug.Log("Navigating the Path");
                yield return new WaitForSeconds(2f); // Wait before the next step

                if (navStatus == NAVIGATIONSTATUS.ERROR)
                {
                    Debug.LogError("Navigation error occurred.");
                    path = null; // Clear the path and retry
                }
                else if (navStatus == NAVIGATIONSTATUS.FINISHED)
                {
                    Debug.Log("Reached the exit.");
                    break; // Stop moving once the zombie reaches the exit
                }
            }

            yield return new WaitForSeconds(waitTime); // Wait before the next step
        }

        Debug.Log("Zombie exiting...");
    }


    internal void SetPlacementSystem(PlacementSystem placementSystem)
    {
        this.placementSystem = placementSystem;
    }
}


/*
public class Zombie : Entity
{
    private Vector3Int TargetTile;
    private int health = 5;

    // Start is called before the first frame update
    void StartRoaming()
    {
        StartCoroutine(AttackNearestCustomer());
    }

    void OnCollisionEnter(Collision other)
    {
        // Check if the colliding object is tagged as "Bullet"
        if (other.gameObject.CompareTag("Bullet"))
        {
            health -= 1;
            Debug.Log($"Zombie took {1} damage. Health remaining: {health}");

            if (health <= 0)
            {
                Destroy(gameObject); // Destroy the zombie
        }

        }
    }
   public void Setup(float speed, ref Grid InputGrid, ref PlacementSystem inputPlacementSystem)
    {
        grid = InputGrid;
        placementSystem = inputPlacementSystem;

        
        GridData floorData = placementSystem.floorData;
        Dictionary<Vector3Int, PlacementData> placedObjects = floorData.GetAllPlacedObjects();


        Dictionary<int, List<Vector3Int>> storedTiles = new Dictionary<int, List<Vector3Int>>();
        foreach (var entry in placedObjects)
        {
            Vector3Int position = entry.Key;
            PlacementData data = entry.Value;

            HashSet<int> ids = data.IDs;

            foreach(int id in ids){
                if(storedTiles.ContainsKey(id)){
                    List<Vector3Int> outList;
                    storedTiles.TryGetValue(id, out outList);
                    if(outList != null){
                        outList.Add(position);
                    }
                    else{
                        outList = new List<Vector3Int>() {position};
                    }
                }
                else {
                    storedTiles.Add(id, new List<Vector3Int>() {position});
                }
            }

            //string ids = string.Join(", ", data.IDs); // Convert IDs to a comma-separated string

            //Debug.Log($"Position: {position}, IDs: [{ids}], PlacedObjectIndex: {data.PlacedObjectIndex}");
        }

        StartCoroutine(AttackNearestCustomer());
    }


    private List<Vector3Int> GetZombiePosList()
    {
        List<Vector3Int> outList = new List<Vector3Int> { };
        Zombie[] Zombies = Zombie.FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in Zombies)
        {
            if (zombie != null && zombie != this)
            {
                if (!zombie.getIntPos().Equals(getIntPos()))
                {
                    outList.Add(zombie.getIntPos());
                }
                if (zombie.path != null && zombie.currentIndex >= 0 && zombie.currentIndex <= zombie.path.Count - 1)
                {
                    outList.Add(zombie.path[zombie.currentIndex]);
                }
                if (zombie.path != null && zombie.currentIndex - 1 >= 0 && zombie.currentIndex - 1 <= zombie.path.Count - 1)
                {
                    outList.Add(zombie.path[zombie.currentIndex - 1]);
                }
                //if (zombie.path != null && zombie.currentIndex - 2 >= 0 && zombie.currentIndex - 2 <= zombie.path.Count - 1)
                //{
                //    outList.Add(zombie.path[zombie.currentIndex - 2]);
                //}
            }
        }
        return outList;
    }
    private void FindRandomLocation()
    {
        placementSystem.GetAllPlacedObjects();
        if (placementSystem == null)
        {
            Debug.LogError("PlacementSystem is not set!");
            return;
        }

        if (gridData == null)
        {
            gridData = placementSystem.floorData;
        }
        else
        {
            Debug.LogError("PlacementSystem is not set!");
            return;
        }

        List<Vector3Int> validPositions = new List<Vector3Int>();
        foreach (var position in gridData.GetAllPlacedObjects().Keys)
        {
            if (!placementSystem.furnitureData.HasObjectAt(position))
            {
                validPositions.Add(position);
            }
        }

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions for random roaming.");
            return;
        }

        Vector3Int randomPosition = validPositions[UnityEngine.Random.Range(0, validPositions.Count)];
        Debug.Log($"Zombie roaming to random position: {randomPosition}");
        FindPath(randomPosition);
    }



    private void FindNearestCustomer()
    {
        if (placementSystem == null)
        {
            print("placementSystem not set");
        }
        else
        {
            if (gridData == null)
            {
                gridData = placementSystem.furnitureData;
            }
            if (placementSystem.furnitureData != null)
            {
                float minDist = float.MaxValue;
                Customer[] Customers = Customer.FindObjectsOfType<Customer>();
                foreach (Customer customer in Customers)
                {
                    if (customer != null)
                    {
                        float tmpDist = Vector3.Distance(customer.getPos(), pos);
                        if (tmpDist < minDist)
                        {
                            minDist = tmpDist;
                            TargetTile = customer.getIntPos();
                            //print("found Customer: " + TargetTile.ToString());
                        }
                    }
                }
            }
        }
    }

    private IEnumerator AttackNearestCustomer()
    {
        while (true)
        {
            if (path == null || path.Count == 0)
            {
                FindNearestCustomer();

                if (TargetTile == Vector3Int.zero) // No customer found
                {
                    Debug.Log("No customer found. Roaming randomly.");
                    FindRandomLocation();
                }
                else
                {
                    Debug.Log($"Customer found at {TargetTile}. Moving to target.");
                    FindPath(TargetTile, GetZombiePosList());
                }
            }
            else if (Vector3.Distance(pos, TargetTile) < 0.5f)
            {
                Debug.Log($"Reached target {TargetTile}.");
                FindNearestCustomer();

                if (TargetTile == Vector3Int.zero)
                {
                    Debug.Log("No new customers found. Roaming randomly.");
                    FindRandomLocation();
                }
                else
                {
                    Debug.Log($"New customer found at {TargetTile}. Moving to target.");
                    FindPath(TargetTile, GetZombiePosList());
                }
            }

            NAVIGATIONSTATUS navStatus = NavigatePath();

            if (navStatus == NAVIGATIONSTATUS.ERROR || navStatus == NAVIGATIONSTATUS.FINISHED)
            {
                Debug.Log("Pathfinding complete. Finding next action.");
                path = null; // Reset path after finishing
            }

            yield return new WaitForSeconds(waitTime);
        }
    }
}*/
