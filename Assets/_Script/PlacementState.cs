using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDatabaseSO database;
    private GridData floorData;
    private GridData furnitureData;
    private GridData turretData;
    private ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          GridData turretData, // Add turretData parameter
                          ObjectPlacer objectPlacer,
                          SoundFeedback soundFeedback)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.turretData = turretData; // Assign turretData
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log("There is a Delay in Placement State?");

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            EventManager.TriggerWarning(); // Call to display warning
            return;
        }

        soundFeedback.PlaySound(SoundType.Place);
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
                                             grid.CellToWorld(gridPosition));

        // Determine which GridData to use based on object ID
        GridData selectedData = GetSelectedData();

        selectedData.AddObjectAt(gridPosition,
                                 database.objectsData[selectedObjectIndex].Size,
                                 database.objectsData[selectedObjectIndex].ID,
                                 index);

        if (database.objectsData[selectedObjectIndex].ID != 0) // Floor tiles don't trigger this
        {
            PlaceSurroundingFloorTiles();
        }

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private GridData GetSelectedData()
    {
        if (database.objectsData[selectedObjectIndex].ID == 0)
        {
            return floorData;
        }
        else if (database.objectsData[selectedObjectIndex].ID >= 6) // Modular turret check
        {
            return turretData;
        }
        else
        {
            return furnitureData;
        }
    }

    public void PlaceSurroundingFloorTiles()
    {
        // Clear all existing floor tiles
        floorData.ClearAllObjects();

        // Iterate through all furniture pieces
        foreach (var furnitureEntry in furnitureData.GetAllPlacedObjects())
        {
            Vector3Int furniturePosition = furnitureEntry.Key;
            PlacementData furnitureDataEntry = furnitureEntry.Value;

            // Calculate adjacent positions for this furniture piece
            List<Vector3Int> adjacentPositions = GetAdjacentPositions(furnitureDataEntry.occupiedPositions);

            // Place floor tiles only in valid positions
            foreach (var position in adjacentPositions)
            {
                if (!furnitureData.HasObjectAt(position))
                {
                    // Check if the tile already exists in the floorData
                    if (floorData.HasObjectAt(position))
                    {
                        // Add the furniture ID to the existing floor tile
                        floorData.GetAllPlacedObjects()[position].AddID(furnitureDataEntry.IDs.First());
                    }
                    else
                    {
                        // Place a new floor tile and add the furniture ID
                        int index = objectPlacer.PlaceObject(database.objectsData[0].Prefab, grid.CellToWorld(position));
                        floorData.AddObjectAt(position, Vector2Int.one, 0, index);
                        floorData.GetAllPlacedObjects()[position].AddID(furnitureDataEntry.IDs.First());
                    }
                }
            }
        }
    }


    private List<Vector3Int> GetAdjacentPositions(List<Vector3Int> occupiedPositions)
    {
        HashSet<Vector3Int> adjacentPositions = new();

        foreach (Vector3Int pos in occupiedPositions)
        {
            Vector3Int[] neighbors = new[]
            {
                pos + new Vector3Int(1, 0, 0),
                pos + new Vector3Int(-1, 0, 0),
                pos + new Vector3Int(0, 0, 1),
                pos + new Vector3Int(0, 0, -1)
            };

            foreach (Vector3Int neighbor in neighbors)
            {
                adjacentPositions.Add(neighbor);
            }
        }

        Debug.Log($"Adjacent positions for furniture: {string.Join(", ", adjacentPositions)}");
        return new List<Vector3Int>(adjacentPositions);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = GetSelectedData();

        if (database.objectsData[selectedObjectIndex].ID >= 6) // Modular turret check
        {
            // Ensure furniture exists beneath turret
            if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
            {
                EventManager.UpdateWarningMessage("Must Place Turrets On Top Of Shelves.");
                return false;
            }
        }

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
