using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfinderUtility
{
    public static float LengthOf(List<Node> path)
    {
        float l = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            l += CalculateDistance(path[i], path[i + 1]);
        }

        return l;
    }

    public static float CalculateDistance(Node n1, Node n2)
    {
        return (n1.Point - n2.Point).magnitude;
    }
}
