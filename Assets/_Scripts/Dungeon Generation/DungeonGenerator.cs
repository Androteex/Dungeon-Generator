using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private readonly DungeonInputManager inputManager;

    private Room[] generatedRooms;

    private void Start()
    {

    }
    private void GenerateDungeon() 
    {
        // Get variables from inputManager

        // Generate a node tree based on the variables

        // Generate rooms and place them on the correct nodes

        // Don't know how to do this yet ???
        // Connect rooms with hallways A*

        // Populate rooms and hallways with WFC
    }
}

public class Room 
{
    public int roomWidth;
    public int roomHeight;

    public RoomType roomType;

    // roomTypeIndex has to be between 0 and 4 to work
    // 0 = Starting room
    // 1 = Encounter room
    // 2 = Treasure room
    // 3 = Shop room
    // 4 = Boss room
    public Room(int width, int height, int roomTypeIndex) 
    {
        roomWidth = width;
        roomHeight = height;

        switch (roomTypeIndex)
        {
            case 0:
                roomType = RoomType.Start;
                break;
            case 1:
                roomType = RoomType.Encounter;
                break;
            case 2:
                roomType = RoomType.Treasure;
                break;
            case 3:
                roomType = RoomType.Shop;
                break;
            case 4:
                roomType = RoomType.Boss;
                break;
            default:
                break;
        }

    }

    public enum RoomType 
    {
        Start,
        Encounter,
        Treasure,
        Shop,
        Boss,
    }
}
  
