using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DungeonGraphGenerator : MonoBehaviour
{
    [SerializeField] private Sprite startSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private Sprite treasureSprite;
    [SerializeField] private Sprite encounterSprite;
    [SerializeField] private Sprite bossSprite;

    #region Graph visualisation
    [SerializeField] private GameObject _nodePrefab;

    public void GenerateVisualGraph(Graph graph)
    {
        GraphNode startNode = graph.GraphNodeList[0];
        InstantiateNodeWithEdges(startNode, 2);
    }

    private void InstantiateNodeWithEdges(GraphNode node, int posMulti)
    {
        var pos = node.PositionInGraph * posMulti;
        DrawGraphNode(node, pos);

        if (node.Neighbors != null)
        {
            foreach (var neighbor in node.Neighbors)
            {
                Vector2 nextPos = neighbor.PositionInGraph * posMulti;
                DrawLineTwoPoints(pos, nextPos);
                InstantiateNodeWithEdges(neighbor, posMulti);
            }
        }
    }

    private void DrawGraphNode(GraphNode node, Vector2 pos)
    {
        GameObject placedNode = Instantiate(_nodePrefab, pos, Quaternion.identity);
        placedNode.name = $"{node.Id} : {node.PositionInGraph}";
        SpriteRenderer sr = placedNode.GetComponent<SpriteRenderer>();

        var roomType = node.roomType;
        switch (roomType)
        {
            case DungeonManager.RoomType.Start:
                sr.sprite = startSprite;
                break;
            case DungeonManager.RoomType.Encounter:
                sr.sprite= encounterSprite;
                break;
            case DungeonManager.RoomType.Treasure:
                sr.sprite = treasureSprite;
                break;
            case DungeonManager.RoomType.Shop:
                sr.sprite = shopSprite;
                break;
            case DungeonManager.RoomType.Boss:
                sr.sprite = bossSprite;
                break;
            case DungeonManager.RoomType.Mystery:
                sr.sprite = mysterySprite;
                break;
            default:
                break;
        }
    }

    private void DrawLineTwoPoints(Vector2 start, Vector2 end)
    {
        Debug.DrawLine(start, end, Color.green, Mathf.Infinity);
    }
    #endregion

    #region Graph generation
    public Graph GenerateGraph()
    {
        // Get necessary variables from DungeonManager
        int roomsInPath = DungeonManager.roomsInPath;
        float encounterPercentage = DungeonManager.encounterPercentage;

        Graph graph = GenerateInitialGraph(roomsInPath);
        graph = GenerateNewNodes(graph);
        graph = SetRoomTypes(graph, encounterPercentage);
        graph = GenerateEdges(graph);
        graph = EditEdges(graph);
        return graph;
    }
    private Graph GenerateInitialGraph(int nodesInPath)
    {
        Graph graph = new Graph();

        // Generate startingroom
        graph.AddNode(0);
        var startNode = graph.GraphNodeList[0];
        startNode.SetPosition(new Vector2(0, 0));
        startNode.SetRoomType(DungeonManager.RoomType.Start);

        // Generate the nodes of the path
        var previousNode = 0;
        for (int i = 1; i < nodesInPath + 1; i++)
        {
            graph.AddNode(i);
            var node = graph.GraphNodeList[i];
            node.SetPosition(new Vector2(0, i));

            previousNode = i;
        }

        // Generate bossroom
        var nodeID = previousNode + 1;
        graph.AddNode(nodeID);
        var bossNode = graph.GraphNodeList[nodeID];
        bossNode.SetPosition(new Vector2(0, nodeID));
        graph.GraphNodeList[nodeID].SetRoomType(DungeonManager.RoomType.Boss);

        return graph;
    }
    private Graph GenerateNewNodes(Graph graph)
    {
        foreach (GraphNode node in graph.GraphNodeList.ToArray())
        {
            Vector2 nodePos = node.PositionInGraph;
            int newNodeID = graph.GraphNodeList.Last().Id + 1;

            if (node.roomType != DungeonManager.RoomType.Start && node.roomType != DungeonManager.RoomType.Boss)
            {
                if (node == graph.GraphNodeList[1])
                {
                    graph = AddNodeAtPos(newNodeID, new Vector2(nodePos.x - 1, nodePos.y), graph);
                    graph = AddNodeAtPos(newNodeID + 1, new Vector2(nodePos.x + 1, nodePos.y), graph);
                }
                else
                {
                    int randPercent = UnityEngine.Random.Range(1, 101);
                    if (randPercent < 50)
                    {
                        int randInt = UnityEngine.Random.Range(0, 2);
                        if (randInt == 0)
                        {
                            nodePos = new Vector2(nodePos.x - 1, nodePos.y);
                        }
                        else
                        {
                            nodePos = new Vector2(nodePos.x + 1, nodePos.y);
                        }
                        graph = AddNodeAtPos(newNodeID, nodePos, graph);
                    }
                    else
                    {
                        graph = AddNodeAtPos(newNodeID, new Vector2(nodePos.x - 1, nodePos.y), graph);
                        graph = AddNodeAtPos(newNodeID + 1, new Vector2(nodePos.x + 1, nodePos.y), graph);
                    }
                }
            }
        }

        return graph;
    }
    private Graph AddNodeAtPos(int newNodeID, Vector2 nodePos, Graph graph)
    {
        graph.AddNode(newNodeID);
        graph.GraphNodeList.Last().SetPosition(nodePos);

        return graph;
    }
    private Graph SetRoomTypes(Graph graph, float encounterPercentage)
    {
        // calculate encounter rooms amount
        int encounterRoomsCount = (int)((graph.NodesCount - 2) * encounterPercentage);
        List<GraphNode> roomsInGraph = new List<GraphNode>();

        foreach (GraphNode node in graph.GraphNodeList)
        {
            if (node.roomType != DungeonManager.RoomType.Start && node.roomType != DungeonManager.RoomType.Boss)
            {
                roomsInGraph.Add(node);
            }
        }

        for (int i = 0; i < encounterRoomsCount; i++)
        {
            int randNode = UnityEngine.Random.Range(0, roomsInGraph.Count - 1);
            var node = roomsInGraph[randNode];
            node.SetRoomType(DungeonManager.RoomType.Encounter);
            roomsInGraph.Remove(node);
        }

        return graph;
    }
    private Graph GenerateEdges(Graph graph)
    {
        foreach (GraphNode node in graph.GraphNodeList)
        {
            Vector2 nodePos = node.PositionInGraph;
            List<GraphNode> children = graph.FindNodesByDepth((int)nodePos.y + 1);

            foreach (GraphNode child in children)
            {
                var childPos = child.PositionInGraph;
                if (childPos.x >= nodePos.x -1 && childPos.x <= nodePos.x + 1)
                {
                    Debug.Log($"Addign Edge from: {node}, to: {child}");
                    graph.AddEdge(node.Id, child.Id);
                }
            }
        }
        return graph;
    }
    private Graph EditEdges(Graph graph)
    {
        for (int depth = 0; depth < graph.NodesCount; depth++)
        {
            List<GraphNode> nodes = graph.FindNodesByDepth(depth);
            GraphNode middleNode = graph.FindNodeByPosition(new Vector2(0, depth));
            nodes.Remove(middleNode);
            
            foreach (GraphNode node in nodes)
            {
                // If node1 and node2 has colliding edges remove one of them
                Vector2 node1Pos = middleNode.PositionInGraph;
                Vector2 node2Pos = node.PositionInGraph;
                GraphNode node3 = graph.FindNodeByPosition(new Vector2(node1Pos.x, node1Pos.y + 1));
                GraphNode node4 = graph.FindNodeByPosition(new Vector2(node2Pos.x, node2Pos.y + 1));

                if (node3 != null && node4 != null)
                {
                    if (middleNode.Neighbors.Contains(node4) && node.Neighbors.Contains(node3))
                    {
                        int randPercent = UnityEngine.Random.Range(1, 101);
                        if (randPercent < 75)
                        {
                            graph.RemoveEdge(middleNode.Id, node4.Id);
                        }
                        else
                        {
                            graph.RemoveEdge(node.Id, node3.Id);
                        }
                    }
                }
            }
        }
        return graph;
    }
    private Graph RemoveCollidingEdges(GraphNode node1, GraphNode node2, Graph graph)
    {
        

        return graph;
    }
    #endregion
}

#region Graph and GraphNode Classes
public class Graph
{
    private List<GraphNode> graphNodeList = new List<GraphNode>();
    public int NodesCount { get { return graphNodeList.Count; } }
    public IList<GraphNode> GraphNodeList { get { return graphNodeList.AsReadOnly(); } }
    
    public bool AddNode(int id)
    {
        if (FindNodeById(id) != null) return false;
        else
        {
            graphNodeList.Add(new GraphNode(id));
            return true;
        }
    }
    public bool AddEdge(int id1, int id2)
    {
        GraphNode gn1 = FindNodeById(id1);
        GraphNode gn2 = FindNodeById(id2);
        if (gn1 == null && gn2 == null)
        {
            return false;
        }
        else if (gn1.Neighbors.Contains(gn2))
        {
            return false;
        }
        else
        {
            gn1.AddNeighbor(gn2);
            return true;
        }
    }
    public GraphNode FindNodeById(int id)
    {
        foreach (GraphNode node in graphNodeList)
        {
            if (node.Id.Equals(id)) return node;
        }
        return null;
    }
    public GraphNode FindNodeByPosition(Vector2 pos)
    {
        foreach(GraphNode node in graphNodeList)
        {
            if (node.PositionInGraph ==  pos) return node;
        }
        return null;
    }
    public List<GraphNode> FindNodesByDepth(int posY)
    {
        List<GraphNode> nodes = new List<GraphNode>();
        foreach (GraphNode node in graphNodeList)
        {
            if (node.PositionInGraph.y == posY) { nodes.Add(node); }
        }
        return nodes;
    }
    public List<GraphNode> FindNodesByBreadth(int posX)
    {
        List<GraphNode> nodes = new List<GraphNode>();
        foreach (GraphNode node in graphNodeList)
        {
            if (node.PositionInGraph.x == posX) { nodes.Add(node); }
        }
        return nodes;
    }
    public bool RemoveNode(int id)
    {
        GraphNode TBR = FindNodeById(id);
        if (TBR != null)
        {
            graphNodeList.Remove(TBR);
            foreach (GraphNode node in graphNodeList)
            {
                node.RemoveNeighbor(TBR);
            }
            return true;
        }
        else return false; 
    }
    public bool RemoveEdge(int n1, int n2)
    {
        GraphNode gn1 = FindNodeById(n1);
        GraphNode gn2 = FindNodeById(n2);
        if (gn1 == null || gn2 == null)
        {
            return false;
        }
        else if (!gn1.Neighbors.Contains(gn2))
        {
            return false;
        }
        else { gn1.RemoveNeighbor(gn2); return true; }
    }
    public bool ClearGraph()
    {
        foreach(GraphNode node in graphNodeList)
        {
            node.RemoveAllNeighbors();
        }
        for (int i = graphNodeList.Count - 1; i >= 0; i--)
        {
            graphNodeList.RemoveAt(i);
        }
        return true;
    }
    public override string ToString()
    {
        StringBuilder nodeString = new StringBuilder();
        for (int i = 0; i < graphNodeList.Count; i++)
        {
            nodeString.Append(graphNodeList[i].ToString());
            if (i < graphNodeList.Count - 1)
            {
                nodeString.Append("\n");
            }
        }
        return nodeString.ToString();
    }
}

public class GraphNode
{
    private List<GraphNode> neighbors;
    private int id;
    private Vector2 positionInGraph;
    public DungeonManager.RoomType roomType;

    public int Id { get { return id; } }
    public IList<GraphNode> Neighbors { get { return neighbors.AsReadOnly(); } }
    public Vector2 PositionInGraph { get { return positionInGraph; } }

    public GraphNode(int id)
    {
        this.id = id;
        neighbors = new List<GraphNode>();
        roomType = DungeonManager.RoomType.Mystery;
    }
    
    public void SetRoomType(DungeonManager.RoomType roomType)
    {
        this.roomType = roomType;
    }
    public void SetPosition(Vector2 positionInGraph)
    {
        this.positionInGraph = positionInGraph;
    }
    public bool AddNeighbor(GraphNode node)
    {
        if (neighbors.Contains(node)) {  return false; }
        else { neighbors.Add(node); return true; }
    }
    public bool RemoveNeighbor(GraphNode node)
    {
        if (neighbors.Contains(node)) 
        {
            return neighbors.Remove(node);
        }
        return false;
    }
    public bool RemoveAllNeighbors()
    {
        foreach (GraphNode node in neighbors)
        {
            neighbors.Remove(node);
        }
        return true;
    }
    public override string ToString()
    {
        StringBuilder nodeString = new StringBuilder();
        nodeString.Append($"[ GraphNode ID: {this.id} with neighbors");
        foreach (GraphNode node in this.neighbors)
        {
            nodeString.Append($" -> {node.id}");
        }
        nodeString.Append(" ]");
        return nodeString.ToString();
    }
}
#endregion
