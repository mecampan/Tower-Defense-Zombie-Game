using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Entity
{
    private List<Vector3Int> shoppingList = new List<Vector3Int>();
    private Vector3Int exitPoint;
    private int currentTargetIndex = 0;



    //public void Setup(List<Vector3Int> shelves, Vector3Int exit, float speed, float wait)
    //{
    //    // Generate a random shopping list from available shelves
    //    int itemsToBuy = Random.Range(1, shelves.Count + 1); // 1 to all shelves
    //    shoppingList = new List<Vector3Int>(shelves);
    //    for (int i = shelves.Count - 1; i >= itemsToBuy; i--)
    //    {
    //        shoppingList.RemoveAt(Random.Range(0, shoppingList.Count));
    //    }

    //    exitPoint = exit;
    //    moveSpeed = speed;
    //    waitTime = wait;

    //    // Start moving toward the first shelf
    //    StartCoroutine(PerformShopping());
    //}

    //private IEnumerator PerformShopping()
    //{
    //    while (currentTargetIndex < shoppingList.Count)
    //    {
    //        Transform targetShelf = shoppingList[currentTargetIndex];
    //        yield return MoveToTarget(targetShelf);

    //        yield return new WaitForSeconds(waitTime);
    //        currentTargetIndex++;
    //    }

    //    yield return MoveToTarget(exitPoint);

    //    Destroy(gameObject);
    //}
}
