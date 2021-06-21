using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class Node
{
    public Node(Vector2Int point)
    {
        this.point = point;
        neighbours = new List<Node>();
    }

    private Node parent;
    private List<Node> neighbours;
    private Vector2Int point;
    private float g = 0;
    private float h = 0;
    private float f = 0;

    public Vector2Int Point
    {
        get => point;
    }

    public List<Node> Neighbours => neighbours;

    public Node Parent
    {
        get => parent;
        set => parent = value;
    }

    public float G
    {
        get => g;
        set => g = value;
    }

    public float H
    {
        get => h;
        set => h = value;
    }

    public float F
    {
        get
        {
            return f;
        }
    }

    public void RecalculateF()
    {
        f = g + h;
    }
}