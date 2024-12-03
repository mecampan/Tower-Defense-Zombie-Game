using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField]
    [Header("Customer Settings")]
    public GameObject customerPrefab;
    public Vector3Int exitPoint; // The exit point after shopping
    [SerializeField]
    public List<Vector3Int> shelves; // List of shelf locations
    public float spawnInterval = 2f; // Time between customer spawns
    [SerializeField]
    public float customerSpeed = 2f; // Speed at which customers move
    public float shelfWaitTime = 3f; // Time customers wait at each shelf
    [SerializeField]
    Grid grid;

    private Vector3Int entryPoint;
    private IEnumerator SpawnCustomers()
    {
        while (true)
        {
            if (entryPoint != null)
            {
                SpawnCustomer();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    private void SpawnCustomer()
    {
        // Create a new customer and set up their shopping routine
        GameObject customer = Instantiate(customerPrefab, grid.CellToWorld(entryPoint), Quaternion.identity);
        Customer customerEntity = customer.AddComponent<Customer>();
        //customerEntity.Setup(shelves, exitPoint, customerSpeed, shelfWaitTime);
    }
}
//public class CustomerManager : MonoBehaviour
//{
//    [SerializeField]
//    [Header("Customer Settings")]
//    public GameObject customerPrefab; 
//    public Transform exitPoint; // The exit point after shopping
//    [SerializeField]
//    public List<Transform> shelves; // List of shelf locations
//    public float spawnInterval = 2f; // Time between customer spawns
//    [SerializeField]
//    public float customerSpeed = 2f; // Speed at which customers move
//    public float shelfWaitTime = 3f; // Time customers wait at each shelf

//    private Transform entryPoint; 

//    private void Start()
//    {
//        StartCoroutine(SpawnCustomers());
//    }

//    public void SetEntryPoint(Transform newEntryPoint)
//    {
//        entryPoint = newEntryPoint;
//    }

//    private IEnumerator SpawnCustomers()
//    {
//        while (true)
//        {
//            if (entryPoint != null)
//            {
//                SpawnCustomer();
//            }
//            yield return new WaitForSeconds(spawnInterval);
//        }
//    }

//    private void SpawnCustomer()
//    {
//        // Create a new customer and set up their shopping routine
//        GameObject customer = Instantiate(customerPrefab, entryPoint.position, Quaternion.identity);
//        CustomerBehavior behavior = customer.AddComponent<CustomerBehavior>();
//        behavior.Setup(shelves, exitPoint, customerSpeed, shelfWaitTime);
//    }
//}

//public class CustomerBehavior : MonoBehaviour
//{
//    private List<Transform> shoppingList = new List<Transform>();
//    private Transform exitPoint;
//    private float moveSpeed;
//    private float waitTime;
//    private int currentTargetIndex = 0;

//    public void Setup(List<Transform> shelves, Transform exit, float speed, float wait)
//    {
//        // Generate a random shopping list from available shelves
//        int itemsToBuy = Random.Range(1, shelves.Count + 1); // 1 to all shelves
//        shoppingList = new List<Transform>(shelves);
//        for (int i = shelves.Count - 1; i >= itemsToBuy; i--)
//        {
//            shoppingList.RemoveAt(Random.Range(0, shoppingList.Count));
//        }

//        exitPoint = exit;
//        moveSpeed = speed;
//        waitTime = wait;

//        // Start moving toward the first shelf
//        StartCoroutine(PerformShopping());
//    }

//    private IEnumerator PerformShopping()
//    {
//        while (currentTargetIndex < shoppingList.Count)
//        {
//            Transform targetShelf = shoppingList[currentTargetIndex];
//            yield return MoveToTarget(targetShelf);

//            yield return new WaitForSeconds(waitTime);
//            currentTargetIndex++;
//        }

//        yield return MoveToTarget(exitPoint);

//        Destroy(gameObject);
//    }

//    private IEnumerator MoveToTarget(Transform target)
//    {
//        while (Vector3.Distance(transform.position, target.position) > 0.1f)
//        {
//            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
//            yield return null;
//        }
//    }
//}
