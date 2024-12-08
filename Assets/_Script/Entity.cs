using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum NAVIGATIONSTATUS
{
    ERROR = 0,
    BETWEENTILES = 1,
    ATTILE = 2,
    FINISHED = 3
}

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected Vector3 pos;
    //[SerializeField]
    Vector3 posOffset = new Vector3(0.0f, 0, 0.0f);
    [SerializeField]
    protected Grid grid;
    [SerializeField]
    protected PlacementSystem placementSystem;
    protected List<Vector3Int> path = null;
    protected GridData gridData = null;
    protected int currentIndex = 0;
    protected const int timer = 100;
    protected int CurrentTime = timer;

    protected float moveSpeed;
    protected float waitTime = 0.003f;
    // Start is called before the first frame update
    void Start()
    {
    }

    public Vector3 getPos()
    {
        return pos;
    }

    public Vector3Int getIntPos()
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x), 0, Mathf.FloorToInt(pos.z));
    }
    public Vector3 getFloatOffset()
    {
        return new Vector3((pos.x - Mathf.Floor(pos.x)) * grid.cellSize.x, 0, (pos.z - Mathf.Floor(pos.z)) * grid.cellSize.z);
    }

    protected NAVIGATIONSTATUS NavigatePath()
    {
        if (path == null || path.Count == 0) return NAVIGATIONSTATUS.ERROR;

        CurrentTime--;
        if (CurrentTime <= 0)
        {
            // Move to the next tile
            currentIndex--;
            if (currentIndex >= 0)
            {
                CurrentTime = timer;
                pos = path[currentIndex];
            }
            if (currentIndex <= 0) return NAVIGATIONSTATUS.FINISHED;
            return NAVIGATIONSTATUS.ATTILE;
        }
        else
        {
            // Smooth movement between tiles
            if (currentIndex >= 1)
            {
                float factor = 1f * CurrentTime / timer;
                Vector3Int currentTile = path[currentIndex];
                Vector3Int nextTile = path[currentIndex - 1];
                pos = Vector3.Lerp(grid.CellToWorld(currentTile), grid.CellToWorld(nextTile), 1f - factor);
            }
        }

        UpdatePos();
        return NAVIGATIONSTATUS.BETWEENTILES;
    }


    protected bool FindPath(Vector3Int target)
    {
        
        if (placementSystem == null)
        {
            print("placementSystem not set");
            return false;
        }
        else
        {
            if (gridData == null)
            {
                gridData = placementSystem.furnitureData;
            }
            if (placementSystem.furnitureData != null)
            {
                if (gridData.IsTileOpen(target))
                {
                    //print("target: " + target.ToString());
                    path = Navagation.FindShortestPath(ref gridData, getIntPos(), target, grid);
                    if (path != null && path.Count > 0)
                    {
                        currentIndex = path.Count - 1;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    protected bool FindPath(Vector3Int target, List<Vector3Int> AvoidCells)
    {
        if (placementSystem == null)
        {
            print("placementSystem not set");
            return false;
        }
        else
        {
            if (gridData == null)
            {
                gridData = placementSystem.furnitureData;
            }
            if (placementSystem.furnitureData != null)
            {
                if (gridData.IsTileOpen(target))
                {
                    //print("target: " + target.ToString());
                    path = Navagation.FindShortestPath(ref gridData, getIntPos(), target, grid, AvoidCells);
                    if (path != null && path.Count > 0)
                    {
                        currentIndex = path.Count - 1;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void UpdatePos()
    {
        transform.position = grid.CellToWorld(getIntPos()) + posOffset + getFloatOffset();
    }


    // Update is called once per frame
    //void Update()
    //{
    //    if (path == null)
    //    {
    //        for (int t = 0; t < 10; t++)
    //        {
    //            Vector3Int tmpTarget = new Vector3Int(math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5), 0, math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5));
    //            if(FindPath(tmpTarget))
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        int tmp = NavigatePath();
    //        if(tmp == 0 || tmp == 2)
    //        {
    //            path = null;
    //        }
    //    }
    //}
}
