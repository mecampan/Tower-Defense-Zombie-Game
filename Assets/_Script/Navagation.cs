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
        int CalcCount = 0;
        List<Vector3Int> outArray = new List<Vector3Int> { };
        if (Start.Equals(End))
        {
            //print("CalcCount1: " + CalcCount);
            return outArray;
        }

        Dictionary<Vector3Int, TileData> SearchedTiles = new();

        List<Vector3Int> tileQueue = new List<Vector3Int> {End};

        {
            TileData tileData = new TileData();
            tileData.pos = End;
            tileData.num = 0;
            SearchedTiles.Add(End, tileData);
        }

        int count = 0;

        for (int i = 0; i < 500; i++)
        {
            //print("int1: " + i);
            count++;
            List<Vector3Int> tmpTileQueue = new List<Vector3Int> { };
            foreach (Vector3Int tile in tileQueue)
            {
                List<Vector3Int> tmpList = getNearbyWalkableTiles(ref data, ref SearchedTiles, tile);
                foreach (Vector3Int tmpTile in tmpList)
                {
                    tmpTileQueue.Add(tmpTile);

                    TileData tileData = new TileData();
                    tileData.pos = tmpTile;
                    tileData.num = count;
                    SearchedTiles.Add(tmpTile, tileData);
                }
            }
            tileQueue.Clear();
            foreach(Vector3Int tile in tmpTileQueue)
            {
                tileQueue.Add(tile);
            }
            if(tileQueue.Count == 0)
            {
                break;
            }
        }

        TileData currentTile = new TileData();
        if(!SearchedTiles.TryGetValue(Start, out currentTile))
        {
            //print("CalcCount1: " + CalcCount);
            return outArray;
        }
        //print("CalcCount1: " + CalcCount);
        for (int i = 0; i < 500; i++)
        {
            //print("int2: " + i);
            TileData minTile = new TileData();
            minTile.num = 999999;
            bool bFoundTile = false;
            foreach (Vector3Int tmpTile in getNearbyTracedTiles(ref data, ref SearchedTiles, currentTile.pos))
            {
                TileData tmpNum = new TileData();
                if(SearchedTiles.TryGetValue(tmpTile, out tmpNum)){
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
        //print("CalcCount2: " + CalcCount);
        outArray.Add(End);
        outArray.Reverse();
        return outArray;
    }

    public static List<Vector3Int> FindShortestPath(ref GridData data, Vector3Int Start, Vector3Int End, Grid grid, List<Vector3Int> AvoidCells)
    {
        int CalcCount = 0;
        List<Vector3Int> outArray = new List<Vector3Int> { };
        if (Start.Equals(End))
        {
            //print("CalcCount1: " + CalcCount);
            return outArray;
        }

        Dictionary<Vector3Int, TileData> SearchedTiles = new();

        List<Vector3Int> tileQueue = new List<Vector3Int> { End };

        {
            TileData tileData = new TileData();
            tileData.pos = End;
            tileData.num = 0;
            SearchedTiles.Add(End, tileData);
        }

        int count = 0;

        for (int i = 0; i < 500; i++)
        {
            //print("int1: " + i);
            count++;
            List<Vector3Int> tmpTileQueue = new List<Vector3Int> { };
            foreach (Vector3Int tile in tileQueue)
            {
                List<Vector3Int> tmpList = getNearbyWalkableTiles(ref data, ref SearchedTiles, tile);
                foreach (Vector3Int tmpTile in tmpList)
                {
                    if (!AvoidCells.Contains(tmpTile))
                    {
                        tmpTileQueue.Add(tmpTile);

                        TileData tileData = new TileData();
                        tileData.pos = tmpTile;
                        tileData.num = count;
                        SearchedTiles.Add(tmpTile, tileData);
                    }
                }
            }
            tileQueue.Clear();
            foreach (Vector3Int tile in tmpTileQueue)
            {
                tileQueue.Add(tile);
            }
            if (tileQueue.Count == 0)
            {
                break;
            }
        }

        TileData currentTile = new TileData();
        if (!SearchedTiles.TryGetValue(Start, out currentTile))
        {
            //print("CalcCount1: " + CalcCount);
            return outArray;
        }
        //print("CalcCount1: " + CalcCount);
        for (int i = 0; i < 500; i++)
        {
            //print("int2: " + i);
            TileData minTile = new TileData();
            minTile.num = 999999;
            bool bFoundTile = false;
            foreach (Vector3Int tmpTile in getNearbyTracedTiles(ref data, ref SearchedTiles, currentTile.pos))
            {
                TileData tmpNum = new TileData();
                if (SearchedTiles.TryGetValue(tmpTile, out tmpNum))
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
        //print("CalcCount2: " + CalcCount);
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
