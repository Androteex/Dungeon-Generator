using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonManager : MonoBehaviour
{
    #region Singleton Logic
    public static DungeonManager Instance { get; private set; }
    private void SingletonAwake()
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
    #endregion

    #region Seed Logic
    public string dungeonSeed = "Default";
    [SerializeField] private int currentSeed = 0;
    private void SeedAwake()
    {
        currentSeed = dungeonSeed.GetHashCode();
        Random.InitState(currentSeed);
    }
    #endregion

    private void Awake()
    {
        SingletonAwake();
        SeedAwake();
    }

    #region Dungeon Systems
    public static DungeonType dungeonType { get; private set; }
    public static DungeonDifficulty dungeonDifficulty { get; private set; }

    public static int roomsInPath { get; private set; }
    public static float encounterPercentage { get; private set; }

    public enum DungeonType
    {
        Small,
        Medium,
        Large
    }

    public enum DungeonDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    public static void AddDungeonProperties(DungeonInputBase input)
    {
        dungeonType = input.dungeonType;
        dungeonDifficulty = input.dungeonDifficulty;

        switch (dungeonType)
        {
            case DungeonType.Small:
                // Small dungeons have 5 rooms in a path
                roomsInPath = 5;
                break;
            case DungeonType.Medium:
                // Small dungeons have 10 rooms in a path
                roomsInPath = 10;
                break;
            case DungeonType.Large:
                // Small dungeons have 15 rooms in a path
                roomsInPath = 15;
                break;
        }

        switch (dungeonDifficulty)
        {
            case DungeonDifficulty.Easy:
                // Easy dungeons have an encounter percentage of 40% (0.4f)
                encounterPercentage = 0.4f;
                break;
            case DungeonDifficulty.Normal:
                // Normal dungeons have an encounter percentage of 50% (0.5f)
                encounterPercentage = 0.5f;
                break;
            case DungeonDifficulty.Hard:
                // Hard dungeons have an encounter percentage of 75% (0.75f)
                encounterPercentage = 0.75f;
                break;
        }
    }

    #region Dungeon Rooms
    public enum RoomType
    {
        Start,
        Encounter,
        Treasure,
        Shop,
        Boss,
        Mystery
    }
    #endregion
    #endregion
}
