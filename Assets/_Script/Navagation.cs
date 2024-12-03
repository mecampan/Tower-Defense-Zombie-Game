using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Navagation : MonoBehaviour
{
    public static List<Vector3Int> FindShortestPath(ref GridData data, Vector3Int Start, Vector3Int End, Grid grid)
    {
        if (data == null)
        {
            Debug.LogError("GridData is null in FindShortestPath.");
            return new List<Vector3Int>();
        }

        List<Vector3Int> outArray = new List<Vector3Int> { };
        if (Start.Equals(End))
        {
            return outArray;
        }

        Dictionary<Vector3Int, TileData> SearchedTiles = new();
        List<Vector3Int> tileQueue = new List<Vector3Int> { End };

        {
            TileData tileData = new TileData
            {
                pos = End,
                num = 0
            };
            SearchedTiles.Add(End, tileData);
        }

        int count = 0;

        for (int i = 0; i < 500; i++)
        {
            count++;
            List<Vector3Int> tmpTileQueue = new List<Vector3Int>();
            foreach (Vector3Int tile in tileQueue)
            {
                List<Vector3Int> tmpList = getNearbyWalkableTiles(ref data, ref SearchedTiles, tile);
                foreach (Vector3Int tmpTile in tmpList)
                {
                    tmpTileQueue.Add(tmpTile);

                    TileData tileData = new TileData
                    {
                        pos = tmpTile,
                        num = count
                    };
                    SearchedTiles.Add(tmpTile, tileData);
                }
            }
            tileQueue.Clear();
            tileQueue.AddRange(tmpTileQueue);
            if (tileQueue.Count == 0)
            {
                break;
            }
        }

        if (!SearchedTiles.TryGetValue(Start, out TileData currentTile))
        {
            return outArray;
        }

        for (int i = 0; i < 500; i++)
        {
            TileData minTile = new TileData { num = 999999 };
            bool bFoundTile = false;
            foreach (Vector3Int tmpTile in getNearbyTracedTiles(ref data, ref SearchedTiles, currentTile.pos))
            {
                if (SearchedTiles.TryGetValue(tmpTile, out TileData tmpNum))
                {
                    if (tmpNum.num < minTile.num)
                    {
                        minTile = tmpNum;
                        bFoundTile = true;
                    }
                }
            }
            if (bFoundTile && minTile.num < currentTile.num)
            {
                outArray.Add(currentTile.pos);
                currentTile = minTile;
            }
            else
            {
                break;
            }
        }
        outArray.Add(End);
        outArray.Reverse();
        return outArray;
    }

    private static List<Vector3Int> getNearbyWalkableTiles(ref GridData data, ref Dictionary<Vector3Int, TileData> SearchedTiles, Vector3Int tile)
    {
        List<Vector3Int> outArray = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++) {
            for (int z = -1; z <= 1; z++)
            {
                Vector3Int tmpTile = new Vector3Int(tile.x + x, tile.y, tile.z + z);
                if ((x == 0 ^ z == 0) && !(SearchedTiles.ContainsKey(tmpTile)) && (tile.x + x <= 5 && tile.x + x >= -5 && tile.z + z <= 5 && tile.z + z >= -5) && data.IsTileOpen(tmpTile))
                {
                    outArray.Add(tmpTile);
                }
            }
        }
        return outArray;
    }
    private static List<Vector3Int> getNearbyTracedTiles(ref GridData data, ref Dictionary<Vector3Int, TileData> SearchedTiles, Vector3Int tile)
    {
        List<Vector3Int> outArray = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3Int tmpTile = new Vector3Int(tile.x + x, tile.y, tile.z + z);
                if ((x == 0 ^ z == 0) && SearchedTiles.ContainsKey(tmpTile))
                {
                    outArray.Add(tmpTile);
                }
            }
        }
        return outArray;
    }
}

struct TileData
{
    public Vector3Int pos;
    public int num;
}
