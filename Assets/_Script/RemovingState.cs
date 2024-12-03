using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private ObjectsDatabaseSO database;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    GridData turretData; // Add turretData
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback; 

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         ObjectsDatabaseSO database,
                         GridData floorData,
                         GridData furnitureData,
                         GridData turretData, // Add turretData parameter
                         ObjectPlacer objectPlacer,
                         SoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.turretData = turretData; // Assign turretData
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;

        // Determine which layer the object belongs to
        if (!turretData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = turretData;
        }
        else if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = furnitureData;
        }

        if (selectedData == null)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            return;
        }

        gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
        if (gameObjectIndex == -1)
        {
            Debug.LogWarning($"No object found at position {gridPosition} to remove.");
            return;
        }

        soundFeedback.PlaySound(SoundType.Remove);
        selectedData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(gameObjectIndex);

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));

        PlaceSurroundingFloorTiles();
    }


    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        // Check all grid data types
        return !(turretData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
                 furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
                 floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
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
                if (!furnitureData.HasObjectAt(position) && !floorData.HasObjectAt(position))
                {
                    // Use ID 0 for floor tiles
                    floorData.AddObjectAt(position, Vector2Int.one, 0, objectPlacer.PlaceObject(database.objectsData[0].Prefab, grid.CellToWorld(position)));
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

        return new List<Vector3Int>(adjacentPositions);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
