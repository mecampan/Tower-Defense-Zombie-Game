using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Zombie : Entity
{
    private Vector3Int TargetTile;
    private int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackNearestCustomer());
    }
    // Health system

    void OnCollisionEnter(Collision other)
    {
        // Check if the colliding object is tagged as "Bullet"
        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1); // Reduce health by 1 per hit
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Zombie took {damage} damage. Health remaining: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Zombie died!");
        Destroy(gameObject); // Destroy the zombie
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
            if (path == null)
            {
                FindNearestCustomer();
                FindPath(TargetTile, GetZombiePosList());
            }
            else if(Vector3.Distance(pos, TargetTile) < 0.5)
            {
                FindNearestCustomer();
            }
            else if (path.Count > 0)
            {
                NAVIGATIONSTATUS tmp = NavigatePath();
                if (tmp == NAVIGATIONSTATUS.ERROR || tmp == NAVIGATIONSTATUS.ATTILE || tmp == NAVIGATIONSTATUS.FINISHED)
                {
                    path = null;
                }
            }
            else
            {
                path = null;
            }
            yield return new WaitForSeconds(waitTime);
        }
        Destroy(gameObject);
    }
}
