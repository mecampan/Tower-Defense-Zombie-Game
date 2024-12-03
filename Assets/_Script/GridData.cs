using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    private ObjectPlacer objectPlacer;
    public GridData(ObjectPlacer objectPlacer)
    {
        this.objectPlacer = objectPlacer;
    }
    private int turretCount = 10;
    private int furnitureCount = 20;

    private readonly int[] FloorID = { 0 };
    private readonly int[] FurnitureID= { 1, 2, 3, 4, 5 };
    private readonly int[] TurretID = { 6 };

    public int GetTurretCount() => turretCount;
    public int GetFurnitureCount() => furnitureCount;

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // Validate placement based on ID
        if (IsTurretID(ID) && turretCount <= 0)
            throw new Exception("No turrets remaining to place.");
        if (IsFurnitureID(ID) && furnitureCount <= 0)
            throw new Exception("No furniture remaining to place.");

        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        
        // Iterate through the positions to be occupied
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.TryGetValue(pos, out var existingData))
            {
                // Add the ID to the existing PlacementData
                existingData.AddID(ID);
            }
            else
            {
                // Create new PlacementData if the position isn't already occupied
                PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
                placedObjects[pos] = data;
            }
        }

        // Update counters
        if (IsTurretID(ID))
            turretCount--;
        else if (IsFurnitureID(ID))
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

    public bool IsTileOpen(Vector3Int gridPosition)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, new Vector2Int(1, 1));

        if (placedObjects.ContainsKey(gridPosition) && (gridPosition.x <= 5 && gridPosition.x >= -5 && gridPosition.z <= 5 && gridPosition.z >= -5))
        {
            return false;
        }

        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition, int ID)
    {
        if (!placedObjects.ContainsKey(gridPosition)) return;

        PlacementData data = placedObjects[gridPosition];

        // Remove the specified ID from this PlacementData
        data.RemoveID(ID);

        // If no IDs remain in the PlacementData, remove the entire entry
        if (data.IDs.Count == 0)
        {
            foreach (var pos in data.occupiedPositions)
            {
                if (placedObjects.ContainsKey(pos))
                    placedObjects.Remove(pos);
            }
        }

        // Update counters based on the removed ID
        if (IsTurretID(ID))
            turretCount++;
        else if (IsFurnitureID(ID))
            furnitureCount++;
    }


    public bool HasObjectAt(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }

    public Dictionary<Vector3Int, PlacementData> GetAllPlacedObjects()
    {
        return placedObjects;
    }

    private bool IsFloorID(int ID) {return Array.Exists(FloorID, floorID => floorID == ID); }
    private bool IsFurnitureID(int ID) {return Array.Exists(FurnitureID, furnitureID => furnitureID == ID); }
    private bool IsTurretID(int ID) {return Array.Exists(TurretID, turretID => turretID == ID); }

    public void ClearAllObjects()
    {
        List<Vector3Int> positionsToClear = new List<Vector3Int>(placedObjects.Keys);
        foreach (var position in positionsToClear)
        {
            int index = GetRepresentationIndex(position);
            if (index != -1)
            {
                placedObjects.Remove(position);
                objectPlacer.RemoveObjectAt(index); // Ensures visual removal
            }
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public HashSet<int> IDs { get; private set; } // Allow multiple IDs
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int initialID, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.IDs = new HashSet<int> { initialID };
        PlacedObjectIndex = placedObjectIndex;
    }

    public void AddID(int id)
    {
        IDs.Add(id);
    }

    public void RemoveID(int id)
    {
        IDs.Remove(id);
    }
}
