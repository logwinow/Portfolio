using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using CustomSerializableTypes;
using System.Linq;
using Custom.SerializationSurrogates;
using Custom.ToolsUtilities;
using Custom.Extensions.Vector;
using UnityEditor.Build.Content;

public class NodesGridWindow : EditorWindow, IHasCustomMenu
{
    [SerializeField]
    private int maxCountOfNodes = 10000;
    [SerializeField]
    private bool isAdvancedSettingsFoldout = false;
    [SerializeField]
    private int gridSize = 1;

    private bool isChoosingStartGenPoint = false;
    private bool isPointsShown = false;
    private List<NodeShort> boundaryNodes;
    private List<NodeShort> nodes;

    [MenuItem("Window/Nodes Grid")]
    private static void ShowWindow()
    {
        var w = EditorWindow.GetWindow(typeof(NodesGridWindow));
        w.titleContent = new GUIContent("Nodes Grid");
        
    }

    void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Reset"), false, Awake);
    }

    private void Awake()
    {
        CustomToolsUtility.LoadEditorWindow(this);
    }

    private void OnDestroy()
    {
        HideGeneratorTool();
        HidePoints();
        nodes?.Clear();
        boundaryNodes?.Clear();
        CustomToolsUtility.SaveEditorWindow(this);
    }

    private string CreateFilePath()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath,
            "Maps"); 
        
        Directory.CreateDirectory(dirPath);
        
        return Path.Combine(dirPath, 
            "mapNodes_" + 
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            + ".dat");
    }

    private void OnGUI()
    {
        if (CustomToolsUtility.StickyButton(ref isChoosingStartGenPoint, "Choose Origin Generation Point"))
        {
            if (isChoosingStartGenPoint)
            {
                SceneView.duringSceneGui += UpdateGeneratorTool;
                Tools.hidden = true;
            }
            else
                HideGeneratorTool();
        }

        if (nodes != null)
        {
            EditorGUILayout.HelpBox($"Nodes count: {nodes.Count}", MessageType.Info);
        }

        gridSize = EditorGUILayout.IntField("Grid size", gridSize);

        if (CustomToolsUtility.StickyButton(ref isPointsShown, "Show points"))
        {
            if (isPointsShown)
            {
                if (nodes == null)
                {
                    Debug.LogWarning("Nodes don't set");
                    isPointsShown = false;

                    return;
                }

                SceneView.duringSceneGui += UpdatePoints;
            }
            else
                HidePoints();
        }

        //if (GUILayout.Button("Delete points"))
        //{
        //    nodesGrid.Nodes.Clear();
        //}

        if (nodes != null && GUILayout.Button("Save"))
        {
            object sdata = new ArrayList()
            {
                gridSize,
                new NodesSerializableData(nodes)
            }; 
            
            BinarySaver.Save(sdata, CreateFilePath(), 
                Vector2IntSerializationSurrogate.SurrogateSelector);

            // AssetDatabase.CreateAsset(assetObj, "Assets/Resources/saves.asset");
            // AssetDatabase.Add
            
        }

        if (GUILayout.Button("Load"))
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            ArrayList ldata = BinarySaver.Load<ArrayList>(
                CreateFilePath(),
                Vector2IntSerializationSurrogate.SurrogateSelector
                );

            gridSize = (int)ldata[0];
            nodes = ((NodesSerializableData)ldata[1]).GetShortNodes();

            stopwatch.Stop();
            Debug.Log($"Loading has taken {stopwatch.ElapsedMilliseconds} ms");
        }

        if (isAdvancedSettingsFoldout = EditorGUILayout.Foldout(isAdvancedSettingsFoldout, "Advanced Settings"))
        {
            EditorGUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(15, 0, 0, 0) });

            maxCountOfNodes = EditorGUILayout.IntField("Max Count of Nodes", maxCountOfNodes);

            EditorGUILayout.EndVertical();
        }

        //if (GUILayout.Button("check"))
        //{
        //    List<NodeEditor> dublicateNode = new List<NodeEditor>();

        //    foreach (var n in nodes)
        //    {
        //        foreach (var _n in nodes)
        //        {
        //            if (n != _n && _n.point == n.point && !dublicateNode.Exists(__n => __n.point == _n.point))
        //            {
        //                dublicateNode.Add(n);
        //                Debug.Log($"Duplicate! {n.point} == {_n.point}");
        //            }
        //        }
        //    }
        //}
    }

    // с помощью pathfinder искать точки, расположенные в границах экрана
    private void UpdatePoints(SceneView sceneView)
    {
        Handles.color = Color.yellow;

        Camera cam = Camera.current;

        if (cam == null)
            return;

        Vector2 _halfsize = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);
        Rect camRect = new Rect((Vector2)cam.transform.position - _halfsize, _halfsize * 2);
        //int i;

        foreach (var n in nodes)
        {
            if (!camRect.Contains(n.Point))
                continue;

            //i = 0;

            foreach (var nei in n.Neighbours)
            {
                //if (++i > 1)
                //    break;
                Handles.DrawLine((Vector2)n.Point, (Vector2)nei.Point);
            }
        }

        sceneView.Repaint();
    }

    private void HidePoints()
    {
        SceneView.duringSceneGui -= UpdatePoints;
        SceneView.RepaintAll();
        Tools.hidden = false;
    }

    private void UpdateGeneratorTool(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(0);

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Vector2 wmpos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

            if (!Physics2D.Raycast(wmpos, Vector2.zero))
            {
                boundaryNodes = new List<NodeShort>() { 
                    new NodeShort(VectorUtility.Vec2ToVec2Int(wmpos, gridSize), 0) 
                };
                nodes = new List<NodeShort>() { boundaryNodes[0] };
                int i = 0;
                NodeShort firstNode;

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                while (boundaryNodes.Count > 0)
                {
                    if (++i > maxCountOfNodes)
                    {
                        Debug.LogWarning("Too much points");
                        break;
                    }

                    firstNode = boundaryNodes[0];
                    boundaryNodes.RemoveAt(0);
                    CreateNeighbours(firstNode);
                }

                stopwatch.Stop();
                Debug.Log($"Generation has taken {stopwatch.ElapsedMilliseconds} ms");

                isChoosingStartGenPoint = false;
                HideGeneratorTool();
                Repaint();
            }
        }
    }

    private void CreateNeighbours(NodeShort node)
    {
        Vector2Int cpos;
        List<NodeShort> newneighbours = new List<NodeShort>();
        List<NodeShort> newboundarynodes = new List<NodeShort>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                cpos = node.Point + new Vector2Int(j, i) * gridSize;

                if (Physics2D.Raycast(cpos, Vector2.zero, 0, 1))
                {
                    continue;
                }

                if (ContainsInNeighbours(cpos, node))
                {
                    continue;
                }

                NodeShort n;

                if (!ContainsInBoundaryNodes(cpos, out n))
                {
                    n = new NodeShort(cpos, nodes.Count);
                    nodes.Add(n);
                    newboundarynodes.Add(n);
                }

                newneighbours.Add(n);
                n.Neighbours.Add(node);
            }
        }

        node.Neighbours.AddRange(newneighbours);
        boundaryNodes.AddRange(newboundarynodes);
    }

    private void HideGeneratorTool()
    {
        Tools.hidden = false;
        SceneView.duringSceneGui -= UpdateGeneratorTool;
    }

    private bool ContainsInNeighbours(Vector2Int point, NodeShort parentNode)
    {
        foreach (var n in parentNode.Neighbours)
            if (point == n.Point)
                return true;

        return false;
    }

    private bool ContainsInBoundaryNodes(Vector2Int point, out NodeShort bNode)
    {
        bNode = boundaryNodes.FirstOrDefault(n => n.Point == point);

        return bNode != null;
    }
}
