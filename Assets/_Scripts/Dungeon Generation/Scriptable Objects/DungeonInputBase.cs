using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Input File", fileName = "New Dungeon Input File")]
public class DungeonInputBase : ScriptableObject
{
    public string dungeonName;
    public DungeonManager.DungeonType dungeonType;
    public DungeonManager.DungeonDifficulty dungeonDifficulty;
}
