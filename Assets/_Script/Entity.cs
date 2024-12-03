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

public class Entity : MonoBehaviour
{
    [SerializeField]
    Vector3 pos;
    [SerializeField]
    Vector3 posOffset;
    [SerializeField]
    Grid grid;
    [SerializeField]
    PlacementSystem placementSystem;
    List<Vector3Int> path = null;
    GridData gridData = null;
    int currentIndex = 0;
    const int timer = 100;
    int CurrentTime = timer;

    protected float moveSpeed;
    protected float waitTime;
    // Start is called before the first frame update
    void Start()
    {
    }

    Vector3Int getIntPos()
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x), 0, Mathf.FloorToInt(pos.z));
    }
    Vector3 getFloatOffset()
    {
        return new Vector3((pos.x - Mathf.Floor(pos.x)) * grid.cellSize.x, 0, (pos.z - Mathf.Floor(pos.z)) * grid.cellSize.z);
    }
    protected int NavigatePath()
    {
        if (placementSystem == null)
        {
            print("placementSystem not set");
            return 0;
        }
        else if(path != null)
        {
            if (gridData == null)
            {
                gridData = placementSystem.furnitureData;
            }
            if (placementSystem.furnitureData != null)
            {
                CurrentTime--;
                if (path != null && CurrentTime <= 0)
                {
                    currentIndex--;
                    if (currentIndex >= 0)
                    {
                        CurrentTime = timer;
                        pos = path[currentIndex];
                    }
                    if (currentIndex <= 0)
                    {
                        return 2;
                    }
                }
                else if (path != null && CurrentTime > 0)
                {
                    if (currentIndex >= 0 && currentIndex - 1 >= 0)
                    {
                        float factor = 1.0f * (1.0f * CurrentTime) / (1.0f * timer);
                        Vector3Int tmpVecInt1 = path[currentIndex];
                        Vector3Int tmpVecInt2 = path[currentIndex - 1];
                        pos = new Vector3((factor * tmpVecInt1.x) + ((1.0f - factor) * tmpVecInt2.x), (factor * tmpVecInt1.y) + ((1.0f - factor) * tmpVecInt2.y), (factor * tmpVecInt1.z) + ((1.0f - factor) * tmpVecInt2.z));
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            UpdatePos();
            return 1;
        }
        return 0;
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

    private void UpdatePos()
    {
        transform.position = grid.CellToWorld(getIntPos()) + posOffset + getFloatOffset();
    }


    // Update is called once per frame
    void Update()
    {
        if (path == null)
        {
            for (int t = 0; t < 10; t++)
            {
                Vector3Int tmpTarget = new Vector3Int(math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5), 0, math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5));
                if(FindPath(tmpTarget))
                {
                    break;
                }
            }
        }
        else
        {
            int tmp = NavigatePath();
            if(tmp == 0 || tmp == 2)
            {
                path = null;
            }
        }
    }

    //void Update()
    //{
    //    transform.position = grid.CellToWorld(getIntPos()) + posOffset + getFloatOffset();
    //    if (placementSystem == null)
    //    {
    //        print("placementSystem not set");
    //    }
    //    else
    //    {
    //        if (gridData == null)
    //        {
    //            gridData = placementSystem.furnitureData;
    //        }
    //        if (placementSystem.furnitureData != null)
    //        {
    //            CurrentTime--;
    //            if (path == null)
    //            {
    //                for (int t = 0; t < 10; t++)
    //                {
    //                    Vector3Int tmpTarget = new Vector3Int(math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5), 0, math.clamp(RandomNumberGenerator.GetInt32(10) - 5, -5, 5));
    //                    if (gridData.IsTileOpen(tmpTarget))
    //                    {
    //                        print("target: " + tmpTarget.ToString());
    //                        path = Navagation.FindShortestPath(ref gridData, getIntPos(), tmpTarget, grid);
    //                        if (path != null && path.Count > 0)
    //                        {
    //                            currentIndex = path.Count - 1;
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //            if (path != null && CurrentTime <= 0)
    //            {
    //                currentIndex--;
    //                if (currentIndex >= 0)
    //                {
    //                    CurrentTime = timer;
    //                    pos = path[currentIndex];
    //                }
    //                if (currentIndex <= 0)
    //                {
    //                    path = null;
    //                }
    //            }
    //            else if (path != null && CurrentTime > 0)
    //            {
    //                if (currentIndex >= 0)
    //                {
    //                    if (currentIndex - 1 >= 0)
    //                    {
    //                        float factor = 1.0f * (1.0f * CurrentTime) / (1.0f * timer);
    //                        Vector3Int tmpVecInt1 = path[currentIndex];
    //                        Vector3Int tmpVecInt2 = path[currentIndex - 1];
    //                        pos = new Vector3((factor * tmpVecInt1.x) + ((1.0f - factor) * tmpVecInt2.x), (factor * tmpVecInt1.y) + ((1.0f - factor) * tmpVecInt2.y), (factor * tmpVecInt1.z) + ((1.0f - factor) * tmpVecInt2.z));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
