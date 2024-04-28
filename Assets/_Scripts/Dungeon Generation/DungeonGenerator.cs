using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonInputBase _dungeonInput;

    private DungeonGraphGenerator _graphGenerator;
    private Graph dungeonGraph;

    private void Start()
    {
        Debug.Log($"Random value: {Random.Range(1, 101)}");
        // Set up essential variables
        _graphGenerator = gameObject.GetComponent<DungeonGraphGenerator>();

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

        // Don't know how to do this yet ???
        // Connect rooms with hallways A*

        // Populate rooms and hallways with WFC
    }
}
  
