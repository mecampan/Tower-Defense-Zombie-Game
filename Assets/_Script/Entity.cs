using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
    const int timer = 1000;
    int CurrentTime = timer;
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

    // Update is called once per frame
    void Update()
    {
        transform.position = grid.CellToWorld(getIntPos()) + posOffset + getFloatOffset();
        if (placementSystem == null)
        {
            print("placementSystem not set");
        }
        else
        {
            if (gridData == null)
            {
                gridData = placementSystem.furnitureData;
            }
            if (placementSystem.furnitureData != null)
            {
                CurrentTime--;
                if (path == null)
                {
                    path = Navagation.FindShortestPath(ref gridData, getIntPos(), Vector3Int.zero, grid);
                    currentIndex = path.Count - 1;
                    if (path != null)
                    {
                        print("path length: " + path.Count);
                    }
                }
                if (path != null && CurrentTime <= 0)
                {
                    currentIndex--;
                    if (currentIndex >= 0)
                    {
                        CurrentTime = 400;
                        pos = path[currentIndex];
                    }
                    if(currentIndex == 0)
                    {
                        path = Navagation.FindShortestPath(ref gridData, getIntPos(), new Vector3Int(-5, 0, -5), grid);
                        currentIndex = path.Count - 1;
                        if (path != null)
                        {
                            print("path length: " + path.Count);
                        }
                    }
                }
                else if (path != null && CurrentTime > 0)
                {
                    if (currentIndex >= 0)
                    {
                        if (currentIndex - 1 >= 0)
                        {
                            float factor = 1.0f * (1.0f * CurrentTime) / 400.0f;
                            Vector3Int tmpVecInt1 = path[currentIndex];
                            Vector3Int tmpVecInt2 = path[currentIndex - 1];
                            pos = new Vector3((factor * tmpVecInt1.x) + ((1.0f - factor) * tmpVecInt2.x), (factor * tmpVecInt1.y) + ((1.0f - factor) * tmpVecInt2.y), (factor * tmpVecInt1.z) + ((1.0f - factor) * tmpVecInt2.z));
                        }
                    }
                }
            }
        }
    }
}
