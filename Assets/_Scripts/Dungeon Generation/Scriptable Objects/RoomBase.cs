using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Dungeon/Room", fileName = "New Room Base")]
public class RoomBase : ScriptableObject
{
    public string roomName;
    public int roomWidth, roomHeight;
    public GameObject roomPrefab;
    public int northConection, westConection, southConection, eastConnection;

    public TileBase[,] GetRoomLayout()
    {
        Tilemap tilemap = roomPrefab.GetComponentInChildren<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        TileBase[,] roomLayout = new TileBase[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Debug.Log($"X: {x} Y: {y} tile: {tile.name}");
                    roomLayout[x, y] = tile;
                }
                else
                {
                    Debug.Log($"X: {x} Y: {y} tile: (null)");
                }
            }
        }
        return roomLayout;
    }
}
