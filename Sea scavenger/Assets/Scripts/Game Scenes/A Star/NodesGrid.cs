using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CustomSerializableTypes;
using Custom.Extensions.Vector;

public class NodesGrid
{
    public NodesGrid(int gridSize)
    {
        nodes = new List<Node>();
        this.gridSize = gridSize;
    }

    private int gridSize;
    private List<Node> nodes;

    public int GridSize => gridSize;
    public List<Node> Nodes => nodes;

    public Vector2Int Vec2ToVec2Int(Vector2 point)
    {
        return VectorUtility.Vec2ToVec2Int(point, gridSize);
    }

    
}
