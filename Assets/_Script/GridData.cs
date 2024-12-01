using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    private int turretCount = 8;
    private int furnitureCount = 12;

    public int GetTurretCount() => turretCount;
    public int GetFurnitureCount() => furnitureCount;

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // Ensure valid placement based on ID
        if (ID >= 5 && turretCount <= 0)
            throw new Exception("No turrets remaining to place.");
        if (ID >= 1 && ID <= 4 && furnitureCount <= 0)
            throw new Exception("No furniture remaining to place.");

        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }

        if (ID >= 5) 
            turretCount--;
        else if (ID >= 1 && ID <= 4) 
            furnitureCount--;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        int minX = -5, maxX = 4;
        int minZ = -5, maxZ = 4;

        if (objectSize == Vector2Int.zero)
            return false;

        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);

        foreach (var pos in positionsToOccupy)
        {
            if (pos.x <= minX || pos.x >= maxX || pos.z <= minZ || pos.z >= maxZ)
                return false;

            if (placedObjects.ContainsKey(pos))
                return false;
        }

        if (furnitureCount <= 0 || turretCount <= 0)
            return false;

        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition)) return;

        PlacementData data = placedObjects[gridPosition];

        foreach (var pos in data.occupiedPositions)
        {
            if (placedObjects.ContainsKey(pos))
                placedObjects.Remove(pos);
        }

        if (data.ID >= 5) 
            turretCount++;
        else if (data.ID >= 1 && data.ID <= 4) 
            furnitureCount++;
    }

    public void RemoveUnsupportedTurrets(GridData furnitureData, GridData turretData, ObjectPlacer objectPlacer)
    {
        List<Vector3Int> unsupportedTurrets = new();

        // Iterate over all turrets
        foreach (var turretEntry in turretData.GetAllPlacedObjects())
        {
            Vector3Int turretPosition = turretEntry.Key;

            // Check if there is no furniture at the same position
            if (!furnitureData.HasObjectAt(turretPosition))
            {
                unsupportedTurrets.Add(turretPosition);
            }
        }

        // Remove unsupported turrets
        foreach (var position in unsupportedTurrets)
        {
            int index = turretData.GetRepresentationIndex(position);
            if (index != -1)
            {
                turretData.RemoveObjectAt(position);
                objectPlacer.RemoveObjectAt(index);
            }
        }
    }

    public bool HasObjectAt(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }

    public Dictionary<Vector3Int, PlacementData> GetAllPlacedObjects()
    {
        return placedObjects;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}