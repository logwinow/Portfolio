using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace CustomSerializableTypes
{
    [Serializable]
    public struct Vector2IntSL
    {
        public Vector2IntSL(Vector2Int v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        private int x;
        private int y;

        public Vector2Int GetVector2Int()
        {
            return new Vector2Int(x, y);
        }
    }

    [Serializable]
    public struct Vector2SL
    {
        public Vector2SL(Vector2 v)
        {
            x = v.x;
            y = v.y;
        }

        private float x;
        private float y;

        public Vector2 GetVector()
        {
            return new Vector2(x, y);
        }
    }

    //[Serializable]
    //public class NodeSL
    //{
    //    public NodeSL(Node node)
    //    {
    //        point = new Vector2IntSL(node.Point);
    //        connectedPoints = new List<Vector2IntSL>();

    //        foreach (var n in node.Neighbours)
    //        {
    //            connectedPoints.Add(new Vector2IntSL(n.Point));
    //        }
    //    }

    //    private List<Vector2IntSL> connectedPoints;
    //    private Vector2IntSL point;

    //    public Vector2Int GetPoint()
    //    {
    //        return point.GetVector2Int();
    //    }

    //    public List<Vector2Int> GetConnectedPoints()
    //    {
    //        return (from p in connectedPoints
    //               select p.GetVector2Int()).ToList();
    //    }
    //}

    [Serializable]
    public class NodesGridSL
    {
        // GRID SIZE - IMPORTANT
        public NodesGridSL(NodesGrid nodesGrid, int gridSize)
        {
            pseudoNodes = new Dictionary<Vector2IntSL, List<Vector2IntSL>>();

            foreach (var n in nodesGrid.Nodes)
            {
                pseudoNodes.Add(new Vector2IntSL(n.Point), (from nei in n.Neighbours select new Vector2IntSL(nei.Point)).ToList());
            }

            this.gridSize = gridSize;
        }

        private Dictionary<Vector2IntSL, List<Vector2IntSL>> pseudoNodes;
        private int gridSize;

        public List<Node> GetNodes()
        {
            List<Node> nodes = (from pn in pseudoNodes.Keys
                               select new Node(pn.GetVector2Int())).ToList();

            foreach (var kv in pseudoNodes)
            {
                Node node = nodes.First(n => n.Point == kv.Key.GetVector2Int());

                foreach (var nc in kv.Value)
                {
                    node.Neighbours.Add(nodes.First(n => n.Point == nc.GetVector2Int()));
                }
            }

            return nodes;
        }

        public NodesGrid GetNodesGrid()
        {
            NodesGrid nodesGrid = new NodesGrid(gridSize);
            nodesGrid.Nodes.AddRange(GetNodes());

            return nodesGrid;
        }
    }

    

    //[Serializable]
    //public class MapBuilderWindowSL
    //{
    //    public MapBuilderWindowSL(List<string> types, List<Block>)
    //}
}
