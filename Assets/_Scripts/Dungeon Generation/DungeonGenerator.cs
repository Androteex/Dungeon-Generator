using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonInputBase _dungeonInput;

    private DungeonGraphGenerator _graphGenerator;
    private Graph dungeonGraph;

    [SerializeField] private RoomBase[] rooms;
    private Tilemap _tilemap;

    private void Start()
    {
        // Set up essential variables
        _graphGenerator = gameObject.GetComponent<DungeonGraphGenerator>();
        _tilemap = gameObject.GetComponentInChildren<Tilemap>();

        // Send appropriate variables to DungeonManager
        DungeonManager.AddDungeonProperties(_dungeonInput);

        // Start generating the dungeon
        GenerateDungeon();
    }
    private void GenerateDungeon() 
    {
        Debug.Log($"Starting generation for: {_dungeonInput.dungeonName}");

        // Generate a node graph with DungeonGraphGenerator
        dungeonGraph = _graphGenerator.GenerateGraph();
        _graphGenerator.GenerateVisualGraph(dungeonGraph);

        // Generate rooms and place them on the correct nodes
        GenerateRoomsOnTilemap();

        // Don't know how to do this yet ???
        // Connect rooms with hallways A*

        // Populate rooms and hallways with WFC
    }

    private void GenerateRoomsOnTilemap()
    {
        GraphNode startNode = dungeonGraph.GraphNodeList[0];

        // Using the first room in the rooms array for testing
        RoomBase roomBase = rooms[0];
        int roomWidth = roomBase.roomWidth;
        int roomHeight = roomBase.roomHeight;

        TileBase[,] roomLayout = roomBase.GetRoomLayout();

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomWidth; y++)
            {
                TileBase tile = roomLayout[x, y];
            }
        }

        GenerateRoomOnGraphNode(roomLayout, roomWidth, roomHeight, dungeonGraph.GraphNodeList[0]);
    }

    //TODO: Place nodes further apart (roomwidth/roomheight) * 2 (multiplier)
    private void GenerateRoomOnGraphNode(TileBase[,] aRoomLayout, int aRoomWidth, int aRoomHeight, GraphNode aNode)
    {
        Vector3 nodePosInWorld = aNode.PositionInGraph;
        Vector3Int nodePosInMap = _tilemap.WorldToCell(nodePosInWorld);

        Vector3Int roomStartPos = new Vector3Int(nodePosInMap.x - (int)(aRoomWidth / 2), nodePosInMap.y + (int)(aRoomHeight / 2));
        Vector3Int roomEndPos = new Vector3Int(nodePosInMap.x + (int)(aRoomWidth / 2), nodePosInMap.y - (int)(aRoomHeight / 2));

        for (int x = roomStartPos.x; x <= roomEndPos.x; x++)
        {
            for (int y = roomStartPos.y; y <= roomEndPos.y; y++)
            {
                _tilemap.SetTile(new Vector3Int(x, y), aRoomLayout[x, y]);
            }
        }
    }
}
  
