
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] 
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;
    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioSource placingItem;
    [SerializeField]
    private AudioSource cantPlaceItem;

    private GridData floorData, furnitureData;
    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
 
    private void Start() 
    {
        stopPlacement();
        floorData = new();
        furnitureData = new();
    }

    public void startPlacement(int ID) 
    {
        stopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectIndex < 0)
        {
            Debug.LogError($"NO ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += stopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if(placementValidity == false)
        {
            cantPlaceItem.Play();
            return;
        }

        placingItem.Play();
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition); 

        placedGameObjects.Add(newObject);   
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
            floorData : 
            furnitureData;
        selectedData.AddObjectAt(gridPosition, 
            database.objectsData[selectedObjectIndex].Size, 
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
            floorData : 
            furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void stopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= stopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if(selectedObjectIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }
}
