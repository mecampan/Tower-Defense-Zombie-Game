using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Navagation
{
    public static Vector3Int[] FindShortestPath(ref GridData data, Vector3Int Start, Vector3Int End)
    {
        Dictionary<Vector3Int, TileData> SearchedTiles = new();

        Vector3Int[] tileQueue = new Vector3Int[] {End};

        int count = 0;

        for (int i = 0; i < 500; i++)
        {
            Vector3Int[] tmpTileQueue = new Vector3Int[] { };
            foreach (Vector3Int tile in tileQueue)
            {
                TileData tileData = new TileData();
                tileData.pos = tile;
                tileData.num = count;
                SearchedTiles.Add(tile, tileData);
            }
            count++;
            foreach (Vector3Int tile in tileQueue)
            {
                foreach (Vector3Int tmpTile in getNearbyTiles(ref data, ref SearchedTiles, tile))
                {
                    tmpTileQueue.Append(tmpTile);
                }
            }
            tileQueue = tmpTileQueue;
        }
        Vector3Int[] outArray = new Vector3Int[] { };

        TileData currentTile = new TileData();
        currentTile.pos = Start;
        currentTile.num = 999999;


        for (int i = 0; i < 500; i++)
        {
            TileData minTile = new TileData();
            minTile.num = 999999;
            bool bFoundTile = false;
            foreach (Vector3Int tmpTile in getNearbyTiles(ref data, ref SearchedTiles, currentTile.pos))
            {
                TileData tmpNum = new TileData();
                if (SearchedTiles.ContainsKey(tmpTile))
                {
                    SearchedTiles.TryGetValue(tmpTile, out tmpNum);
                    if(tmpNum.num < minTile.num)
                    {
                        minTile = tmpNum;
                        bFoundTile = true;
                    }
                }
            }
            if (bFoundTile)
            {
                outArray.Append(currentTile.pos);
                if(currentTile.pos.Equals(End) || currentTile.num == 0)
                {
                    return outArray;
                }
                currentTile = minTile;
            }

        }
        return outArray;
    }
    private static Vector3Int[] getNearbyTiles(ref GridData data, ref Dictionary<Vector3Int, TileData> SearchedTiles, Vector3Int tile)
    {
        Vector3Int[] outArray = new Vector3Int[] { };
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++)
            {
                if((x == 0 ^ y == 0) && data.CanPlaceObjectAt(new Vector3Int(tile.x + x, tile.y + y, tile.z), new Vector2Int(1, 1)) && !SearchedTiles.ContainsKey(new Vector3Int(tile.x + x, tile.y + y, tile.z)))
                {
                    outArray.Append(new Vector3Int(tile.x + x, tile.y + y, tile.z));
                }
            }
        }
        return null;
    }
}

struct TileData
{
    public Vector3Int pos;
    public int num;
}
