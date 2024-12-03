using UnityEngine;

public class ShopperSpawner : MonoBehaviour
{
    public GameObject shopperPrefab;
    public Grid grid;
    public Vector3Int spawnPosition;

    public void SpawnShopper()
    {
        if (shopperPrefab == null || grid == null)
        {
            Debug.LogError("ShopperSpawner is missing required references.");
            return;
        }

        Vector3 worldPosition = grid.CellToWorld(spawnPosition);
        GameObject newShopper = Instantiate(shopperPrefab, worldPosition, Quaternion.identity);

        Shopper shopperScript = newShopper.GetComponent<Shopper>();
        if (shopperScript != null)
        {
            shopperScript.Initialize(spawnPosition, grid);
        }

        Debug.Log($"Spawned shopper at grid cell {spawnPosition}");
    }
}
