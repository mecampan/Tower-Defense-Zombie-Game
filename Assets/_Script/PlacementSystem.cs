using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    public GridData floorData, furnitureData, turretData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private SoundFeedback soundFeedback;

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new(objectPlacer);
        furnitureData = new(objectPlacer);
        turretData = new(objectPlacer);

        UpdateUI();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           turretData,
                                           objectPlacer,
                                           soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, 
                                          preview,
                                          database,
                                          floorData, 
                                          furnitureData, 
                                          turretData, 
                                          objectPlacer, 
                                          soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    public void StopPlacement()
    {
        soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void UpdateUI()
    {
        uiManager.UpdateTurretCounter(turretData.GetTurretCount());
        uiManager.UpdateFurnitureCounter(furnitureData.GetFurnitureCount());
    }

    private void Update()
    {
        if (buildingState != null)
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            if (lastDetectedPosition != gridPosition)
            {
                buildingState.UpdateState(gridPosition);
                lastDetectedPosition = gridPosition;
            }
        }

        UpdateUI();
    }

    public void GetAllPlacedObjects()
    {
        Debug.Log("Floor Data:");
        LogPlacedObjects(floorData.GetAllPlacedObjects());

        Debug.Log("Furniture Data:");
        LogPlacedObjects(furnitureData.GetAllPlacedObjects());

        Debug.Log("Turret Data:");
        LogPlacedObjects(turretData.GetAllPlacedObjects());
    }

    private void LogPlacedObjects(Dictionary<Vector3Int, PlacementData> placedObjects)
    {
        foreach (var entry in placedObjects)
        {
            Vector3Int position = entry.Key;
            PlacementData data = entry.Value;

            string ids = string.Join(", ", data.IDs); // Convert IDs to a comma-separated string
            Debug.Log($"Position: {position}, IDs: [{ids}], PlacedObjectIndex: {data.PlacedObjectIndex}");
        }
    }

}
