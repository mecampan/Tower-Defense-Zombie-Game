using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Zombie : Entity
{
    private Vector3Int TargetTile;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackNearestCustomer());
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
                    float tmpDist = Vector3.Distance(customer.getPos(), pos);
                    if (customer != null && tmpDist < minDist)
                    {
                        minDist = tmpDist;
                        TargetTile = customer.getIntPos();
                        //print("found Customer: " + TargetTile.ToString());
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
                FindPath(TargetTile);
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
