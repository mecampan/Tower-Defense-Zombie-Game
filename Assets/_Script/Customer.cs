using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.Mathematics;
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

    private IEnumerator PerformShopping()
    {
        while (currentTargetIndex < shoppingList.Count)
        {
            if (path == null)
            {
                Vector3Int targetShelf = shoppingList[currentTargetIndex];
                if (FindPath(targetShelf));
                {
                    //print("found new shelf: " + targetShelf.ToString());
                }
            }
            else
            {
                Vector3Int targetShelf = shoppingList[currentTargetIndex];
                NAVIGATIONSTATUS tmp = NavigatePath();
                if (tmp == NAVIGATIONSTATUS.ERROR || (tmp == NAVIGATIONSTATUS.FINISHED && getIntPos().Equals(targetShelf)))
                {
                    if (getIntPos().Equals(targetShelf))
                    {
                        //print("reached shelf: " + targetShelf.ToString());
                        currentTargetIndex++;
                    }
                    path = null;
                }
                else if ((tmp == NAVIGATIONSTATUS.ATTILE || tmp == NAVIGATIONSTATUS.FINISHED) && gridData != null)
                {
                    path = null;
                    if (getIntPos().Equals(targetShelf))
                    {
                        //print("reached shelf: " + targetShelf.ToString());
                        currentTargetIndex++;
                    }
                    Zombie[] Zombies = Zombie.FindObjectsOfType<Zombie>();
                    float MinDist = float.MaxValue;
                    foreach (Zombie zombie in Zombies)
                    {
                        float tmpDist = Vector3Int.Distance(getIntPos(), zombie.getIntPos());
                        if (tmpDist < MinDist)
                        {
                            MinDist = tmpDist;
                        }
                    }
                    if(MinDist <= 4)
                    {
                        float maxDist = 0;
                        Vector3Int maxTile = getIntPos();
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int z = -2; z <= 2; z++)
                            {
                                Vector3Int tmpTile = new Vector3Int(getIntPos().x + x, 0, getIntPos().z + z);
                                foreach (Zombie zombie in Zombies)
                                {
                                    float tmpDist = Vector3Int.Distance(tmpTile, zombie.getIntPos());
                                    if (tmpDist > maxDist && Navagation.IsOnGrid(tmpTile) && gridData != null && gridData.IsTileOpen(tmpTile))
                                    {
                                        if (FindPath(tmpTile))
                                        {
                                            maxDist = tmpDist;
                                            maxTile = new Vector3Int(tmpTile.x, 0, tmpTile.z);
                                        }
                                    }
                                }
                            }
                        }
                        if(maxDist > 0)
                        {
                            FindPath(maxTile);
                        }
                    }
                }
                //List<Vector3Int> outList = new List<Vector3Int> { };
                //Zombie[] Zombies = Zombie.FindObjectsOfType<Zombie>();
                //foreach (Zombie zombie in Zombies)
                //{
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
                NAVIGATIONSTATUS tmp = NavigatePath();
                if (tmp == NAVIGATIONSTATUS.ERROR || tmp == NAVIGATIONSTATUS.FINISHED)
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
