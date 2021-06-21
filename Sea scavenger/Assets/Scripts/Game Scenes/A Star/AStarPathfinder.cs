using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Custom.SerializationSurrogates;

public class AStarPathfinder
{
    public AStarPathfinder()
    {
        path = new List<Node>();

        string spath = System.IO.Path.Combine(Application.streamingAssetsPath,
            "Maps", 
            "mapNodes_2.dat");

        if (!System.IO.File.Exists(spath))
            return;

        var ldata = BinarySaver.Load<ArrayList>(spath, Vector2IntSerializationSurrogate.SurrogateSelector);
        nodesGrid = new NodesGrid((int)ldata[0]);
        nodesGrid.Nodes.AddRange(((NodesSerializableData)ldata[1]).GetNodes());
    }

    public AStarPathfinder(int gCoeff) : this()
    {
        this.gCoeff = gCoeff;
    }

    private List<Node> path;
    private NodesGrid nodesGrid;
    private int gCoeff = 1;
    private float _length;
    private bool _lengthCached = false;

    public List<Node> Path => path;
    public float Length
    {
        get
        {
            if (!_lengthCached)
            {
                _length = PathfinderUtility.LengthOf(path);
                _lengthCached = true;
            }

            return _length;
        }
    }

    public NodesGrid NodesGrid => nodesGrid;

    public Node FindNode(Vector3 position)
    {
        Vector2Int pInt = nodesGrid.Vec2ToVec2Int(position);

        return nodesGrid.Nodes.Find(n => n.Point == pInt);
    }

    public void FindPath(Node start, Node end, bool isDirectionImportant = true)
    {
        start.Parent = null;
        start.G = 0;
        start.H = CalculateH(start, end);

        List<Node> closed = new List<Node>();
        List<Node> open = new List<Node>();
        Node current = start;
        Node lowestFNode = start;

        open.Add(start);
        while (open.Count > 0)
        {
            //Node current = FindNodeWithLowestF(open);
            current = lowestFNode;

            if (current == end)
            {
                ReconstructPath(end, !isDirectionImportant);

                return;
            }

            open.Remove(current);
            closed.Add(current);

            lowestFNode = open.FirstOrDefault();

            foreach (var n in current.Neighbours)
            {
                if (closed.Contains(n))
                    continue;

                float newG = current.G + CalculateG(current, n);

                if (!open.Contains(n))
                {
                    n.G = newG;
                    n.H = CalculateH(n, end);
                    n.RecalculateF();

                    open.Add(n);
                    n.Parent = current;
                }
                else if (n.G > newG)
                {
                    n.G = newG;
                    n.RecalculateF();

                    n.Parent = current;
                }

                if (lowestFNode == null || lowestFNode.F > n.F)
                    lowestFNode = n;
            }    
        }
    }

    public void FindPath(Vector3 start, Vector3 end, bool isDirectionImportant = true)
    {
        FindPath(FindNode(start), FindNode(end), isDirectionImportant);
    }
    
    public void FindPath(Node start, Vector3 end, bool isDirectionImportant = true)
    {
        FindPath(start, FindNode(end), isDirectionImportant);
    }

    public Task FindPathAsync(Node start, Node end, bool isDirectionImportant = true)
    {
        return Task.Run(() => FindPath(start, end, isDirectionImportant));
    }

    public Task FindPathAsync(Node start, Vector3 end, bool isDirectionImportant = true)
    {
        return Task.Run(() => FindPath(start, FindNode(end), isDirectionImportant));
    }

    private void ReconstructPath(Node end, bool inverseDirection)
    {
        path.Clear();

        for (Node n = end; n != null; n = n.Parent)
        {
            path.Add(n);  
        }

        if (!inverseDirection)
            path.Reverse();

        _lengthCached = false;
    }

    private int CalculateG(Node n1, Node n2)
    {
        return CalculateSqrDistance(n1, n2, gCoeff);
    }

    private int CalculateH(Node current, Node end)
    {
        return CalculateSqrDistance(current, end, 1);
    }

    private int CalculateSqrDistance(Node n1, Node n2, int coeff)
    {
        return (n1.Point - n2.Point).sqrMagnitude * coeff;
    }
}
