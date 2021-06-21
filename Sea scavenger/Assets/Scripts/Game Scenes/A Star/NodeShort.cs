using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeShort
{
    public NodeShort(Vector2Int point, int id)
    {
        this.point = point;
        this.id = id;
        neighbours = new List<NodeShort>();
    }

    private Vector2Int point;
    private List<NodeShort> neighbours;
    private int id;

    public List<NodeShort> Neighbours => neighbours;
    public Vector2Int Point => point;
    public int ID => id;
}
