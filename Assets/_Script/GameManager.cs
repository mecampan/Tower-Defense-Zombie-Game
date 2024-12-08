using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject textInfo;
    [SerializeField] private GameObject startPlayModeButton;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private ZombieManager zombieManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private ratingsystem ratingsystem;

    public void StartPlayMode()
    {
        if(CreatedOneOfEachShelf())
        {
            uIManager.ShowDialog(
                () =>
                {
                    // Ensure popup is destroyed before hiding other UI
                    if (panel != null)
                    {
                        panel.SetActive(false);
                        textInfo.SetActive(false);
                    }

                    if (startPlayModeButton != null)
                    {
                        startPlayModeButton.SetActive(false);
                    }

                    placementSystem.StopPlacement();
                    BeginPlayMode();
                },
                () =>
                {
                }
            );
        }
        else
        {
            EventManager.UpdateWarningMessage("Must place one of each type of food shelf to continue");
            EventManager.TriggerWarning();
        }
    }

    private bool CreatedOneOfEachShelf()
    {
        HashSet<int> uniqueIds = new HashSet<int>();
        
        GridData furnitureData = placementSystem.furnitureData;
        Dictionary<Vector3Int, PlacementData> placedObjects = furnitureData.GetAllPlacedObjects();

        foreach (var entry in placedObjects)
        {
            Vector3Int position = entry.Key;
            PlacementData data = entry.Value;

            foreach (int id in data.IDs)
            {
                uniqueIds.Add(id);
            }
        }

        // Check if the HashSet contains all the required IDs
        int[] requiredIds = { 1, 2, 3, 4, 5 };
        foreach (int requiredId in requiredIds)
        {
            if (!uniqueIds.Contains(requiredId))
            {
                return false; // If any required ID is missing, return false
            }
        }

        return true;
    }


    private void BeginPlayMode()
    {
        customerManager.BeginCustomerSpawner();
        zombieManager.BeginZombieSpawner();
        ratingsystem.SetCustomerCountVisibility(true);
    }
}
