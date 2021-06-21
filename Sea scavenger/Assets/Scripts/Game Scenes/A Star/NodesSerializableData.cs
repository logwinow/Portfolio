using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NodesSerializableData
{
    public NodesSerializableData(List<NodeShort> nodes)
    {
        points = new Vector2Int[nodes.Count];
        neighboursIndices = new int[nodes.Count][];

        NodeShort curNode;
        int i, j;

        for (i = 0; i < nodes.Count; i++)
        {
            curNode = nodes[i];
            points[i] = curNode.Point;
            neighboursIndices[i] = new int[curNode.Neighbours.Count];

            for (j = 0; j < curNode.Neighbours.Count; j++)
            {
                neighboursIndices[i][j] = curNode.Neighbours[j].ID;
            }
        }
    }

    private Vector2Int[] points;
    private int[][] neighboursIndices;

    public List<NodeShort> GetShortNodes()
    {
        List<NodeShort> nodes = new List<NodeShort>();
        NodeShort curNode;
        int[] curIndices;
        int i, j;

        for (i = 0; i < points.Length; i++)
        {
            curNode = new NodeShort(points[i], i);
            nodes.Add(curNode);
        }

        for (i = 0; i < points.Length; i++)
        {
            curNode = nodes[i];
            curIndices = neighboursIndices[i];

            for (j = 0; j < curIndices.Length; j++)
            {
                curNode.Neighbours.Add(nodes[curIndices[j]]);
            }
        }

        return nodes;
    }

    public List<Node> GetNodes()
    {
        List<Node> nodes = new List<Node>();
        Node curNode;
        int[] curIndices;
        int i, j;

        for (i = 0; i < points.Length; i++)
        {
            curNode = new Node(points[i]);
            nodes.Add(curNode);
        }

        for (i = 0; i < points.Length; i++)
        {
            curNode = nodes[i];
            curIndices = neighboursIndices[i];

            for (j = 0; j < curIndices.Length; j++)
            {
                curNode.Neighbours.Add(nodes[curIndices[j]]);
            }
        }

        return nodes;
    }
}
