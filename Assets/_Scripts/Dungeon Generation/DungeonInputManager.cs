using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInputManager : MonoBehaviour
{
    public static DungeonInputManager Instance { get; private set; }

    // Min and max size a room will be
    public int minRoomSize { get; private set; }
    public int maxRoomSize { get; private set; }

    // Room ammount refers to how many rooms of that type will exist
    public int bossRoomAmmount { get; private set; }
    public int encounterRoomAmmount { get; private set; }
    public int treasureRoomAmmount { get; private set; }
    public int shopRoomAmmount { get; private set; }

    private void Awake() 
    {
        // This makes it so that there will only be one instance of this object in a given scene (makes it a singelton)
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void SetVariables(int min, int max, int treasureRooms, int shopRooms, int encounterRooms, int bossRooms)
    {
        minRoomSize = min;
        maxRoomSize = max;
        treasureRoomAmmount = treasureRooms;
        shopRoomAmmount = shopRooms;
        encounterRoomAmmount = encounterRooms;
        bossRoomAmmount = bossRooms;
    }
}
