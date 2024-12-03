using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Customer : Entity
{
    [SerializeField]
    [Header("Customer Testing")]
    private List<Vector3Int> shoppingList = new List<Vector3Int>();
    [SerializeField]
    [Header("Customer Testing")]
    private Vector3Int exitPoint;
    private int currentTargetIndex = 0;
    [SerializeField]
    [Header("Customer Testing")]
    private bool bShopping = false;


    private void Start()
    {
        StartCoroutine(PerformShopping());
    }

    public void Setup(List<Vector3Int> shelves, Vector3Int exit, float speed, float wait)
    {
        // Generate a random shopping list from available shelves
        int itemsToBuy = Random.Range(1, shelves.Count + 1); // 1 to all shelves
        shoppingList = new List<Vector3Int>(shelves);
        for (int i = shelves.Count - 1; i >= itemsToBuy; i--)
        {
            shoppingList.RemoveAt(Random.Range(0, shoppingList.Count));
        }

        exitPoint = exit;
        moveSpeed = speed;
        waitTime = wait;

        // Start moving toward the first shelf
        StartCoroutine(PerformShopping());
    }

    private IEnumerator PerformShopping()
    {
        while (currentTargetIndex < shoppingList.Count)
        {
            if (path == null)
            {
                Vector3Int targetShelf = shoppingList[currentTargetIndex];
                if (FindPath(targetShelf))
                {
                    print("found new shelf: " + targetShelf.ToString());
                }
            }
            else
            {
                int tmp = NavigatePath();
                if (tmp == 0 || tmp == 2)
                {
                    Vector3Int targetShelf = shoppingList[currentTargetIndex];
                    print("reached shelf: " + targetShelf.ToString());
                    path = null;
                    currentTargetIndex++;
                }
            }
            yield return new WaitForSeconds(waitTime);
        }

        while (true)
        {
            if (path == null)
            {
                FindPath(exitPoint);
            }
            else
            {
                int tmp = NavigatePath();
                if (tmp == 0 || tmp == 2)
                {
                    if (pos.Equals(exitPoint))
                    {
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(waitTime);
        }

        Destroy(gameObject);
    }
}
