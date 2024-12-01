using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    GridData turretData; // Add turretData
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback; 

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         GridData turretData, // Add turretData parameter
                         ObjectPlacer objectPlacer,
                         SoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
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
        else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = floorData;
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

        // Remove unsupported turrets when furniture is removed
        if (selectedData == furnitureData)
        {
            turretData.RemoveUnsupportedTurrets(furnitureData, turretData, objectPlacer);
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }



    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        // Check all grid data types
        return !(turretData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
                 furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
                 floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
