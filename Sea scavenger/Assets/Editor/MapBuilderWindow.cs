using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.IO;
using UnityEditor.ShortcutManagement;
using Custom.ToolsUtilities;
using Custom.SerializationSurrogates;
using Custom.Extensions.Vector;
using UnityEngine.UI;

public class MapBuilderWindow : EditorWindow
{
    private const string folderName = "MapBuilder Blocks [SYSTEM]";

    private bool isBuilding = false;
    private bool setCollider = true;
    private InspectorBlock selectedBlock;
    private Transform folder;
    private int gridSize = 2;
    private GameObject exampleGO;
    [SerializeField]
    private List<InspectorBlock> inspBlocks;
    private InspectorBlock lastBlock;
    private Texture2D blocksAtlasTex2D;
    private string pathToAtlasPath;
    private bool isBlocksFoldout = true;
    private string path;
    [SerializeField]
    private List<Block> blocks;
    private Vector2 scrollPos;

    [MenuItem("Window/Map Builder")]
    private static void ShowWinodw()
    {
        var w = EditorWindow.GetWindow(typeof(MapBuilderWindow));
        w.titleContent = new GUIContent("Map Builder");
    }

    [Shortcut("MapBuilderWindow/Building Mode", 
        typeof(MapBuilderWindow), KeyCode.B)]
    private static void ShortcutBuilding(ShortcutArguments args)
    {
        MapBuilderWindow mbw = (MapBuilderWindow)args.context;
        if (mbw.isBuilding)
        {
            mbw.HideBuildingTool();
            mbw.isBuilding = false;
        }
        else
        {
            mbw.ActivateBuildingTool();
            mbw.isBuilding = true;
        }
    }

    [Shortcut("MapBuilderWindow/Set Collider",
        typeof(MapBuilderWindow), KeyCode.C)]
    private static void ShortcutSetCollider(ShortcutArguments args)
    {
        MapBuilderWindow mbw = (MapBuilderWindow)args.context;
        mbw.setCollider = !mbw.setCollider;
    }

    private void Awake()
    {
        if ((folder = GameObject.Find(folderName)?.transform ?? null) == null)
        {
            folder = (new GameObject(folderName)).transform;
        }

        inspBlocks = new List<InspectorBlock>();
        lastBlock = new InspectorBlock();
        blocks = new List<Block>();

        path = Path.Combine(Application.dataPath, "Serialized Data");
        Directory.CreateDirectory(path);
        path = Path.Combine(path, "Editor");
        Directory.CreateDirectory(path);
        path = Path.Combine(path, "Map");
        Directory.CreateDirectory(path);
        path = Path.Combine(path, "Blocks");
        Directory.CreateDirectory(path);
        pathToAtlasPath = Path.Combine(path, "PathToBlocksAtlas.dat");
        path = Path.Combine(path, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_MapBuilderWindowInspector.dat");
    }
    private void OnDisable()
    {
        HideBuildingTool();
    }

    private void OnGUI()
    {
        //CheckShortcuts();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        if (CustomToolsUtility.StickyButton(ref isBuilding, "Building"))
        {
            if (isBuilding)
            {
                ActivateBuildingTool();
            }
            else
            {
                HideBuildingTool();
            }
        }

        string msg;
        if (selectedBlock != null && selectedBlock.Sprite != null)
        {
            msg = $"Selected block: \"{selectedBlock.Type}\"";
        }
        else
            msg = "Block doesn't selected";

        EditorGUILayout.HelpBox(msg, MessageType.Info);

        setCollider = EditorGUILayout.Toggle("Set Collider", setCollider);
        gridSize = EditorGUILayout.IntField("Grid size", gridSize);

        if ((isBlocksFoldout = EditorGUILayout.Foldout(isBlocksFoldout, "Blocks", true)))
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i <= inspBlocks.Count; i++)
            {
                InspectorBlock b = i == inspBlocks.Count ? lastBlock : inspBlocks[i];

                EditorGUILayout.BeginHorizontal();

                if (i != inspBlocks.Count)
                {
                    if (GUILayout.Button("Select"))
                    {
                        selectedBlock = b;
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        inspBlocks.RemoveAt(i);
                        break;
                    }
                }

                EditorGUILayout.BeginVertical();

                b.Type = EditorGUILayout.TextField(b.Type);

                EditorGUILayout.BeginHorizontal();

                if (b.Type != "")
                {
                    var srtOrdrsNames = (from l in SortingLayer.layers select l.name).ToList();

                    b.SortingLayer = srtOrdrsNames.ElementAt(
                        EditorGUILayout.Popup(srtOrdrsNames.IndexOf(b.SortingLayer), 
                        srtOrdrsNames.ToArray()));
                    b.SortingOrder = EditorGUILayout.IntField(b.SortingOrder);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                if (b.Type != "")
                    b.Sprite = (Sprite)EditorGUILayout.ObjectField(b.Sprite, typeof(Sprite), false);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                if (i == inspBlocks.Count)
                {
                    if (b.Sprite != null)
                    {
                        inspBlocks.Add(b);
                        lastBlock = new InspectorBlock();

                        //Debug.Log((Sprite)((AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(blocksAtlasTex2D)))[0]));
                    }
                }
            }

            EditorGUI.indentLevel--;
        }

        blocksAtlasTex2D = (Texture2D)EditorGUILayout.ObjectField("Blocks Atlas", blocksAtlasTex2D, typeof(Texture2D), false);

        if (blocks != null && blocks.Count > 0 && GUILayout.Button("Save"))
        {
            UpdateBlocks();
            BinarySaver.Save(new MapBuilderWindowSL(inspBlocks, blocks, gridSize), path, 
                Vector2SerializationSurrogate.SurrogateSelector);

            if (blocksAtlasTex2D != null)
            {
                BinarySaver.Save(AssetDatabase.GetAssetPath(blocksAtlasTex2D), pathToAtlasPath);
            }
        }

        if (GUILayout.Button("Load"))
        {
            if (blocksAtlasTex2D == null)
            {
                object obj = BinarySaver.Load(pathToAtlasPath);

                if (obj == null)
                {
                    Debug.LogWarning("Sprites atlas doesn't set");
                    return;
                }

                blocksAtlasTex2D = AssetDatabase.LoadAssetAtPath<Texture2D>((string)obj);
            }
            
            if (!File.Exists(path))
            {
                Debug.LogWarning($"File at path \"{path}\" doesn't found");
                return;
            }

            DestroyAllBlocks();

            var ldata = ((MapBuilderWindowSL)BinarySaver.Load(path, 
                Vector2SerializationSurrogate.SurrogateSelector)).GetMapBuilderWindowParameters(blocksAtlasTex2D);

            inspBlocks = ldata.InspectorBlocks;
            blocks = ldata.Blocks;
            gridSize = ldata.GridSize;
        }

        if (blocks == null)
            return;

        EditorGUILayout.HelpBox($"Blocks Count: {blocks.Count}", MessageType.Info);

        if (GUILayout.Button("Delete Dublicates"))
        {
            int i, j;

            for (i = 0; i < blocks.Count; i++)
            {
                for (j = i + 1; j < blocks.Count; )
                {
                    if (blocks[i].GO.transform.position == blocks[j].GO.transform.position)
                    {
                        DestroyImmediate(blocks[j].GO);
                        blocks.RemoveAt(j);
                    }
                    else
                        j++;
                }
            }
        }

        if (GUILayout.Button("Destroy All"))
        {
            DestroyAllBlocks();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DestroyAllBlocks()
    {
        while (folder.childCount > 0)
            DestroyImmediate(folder.GetChild(0).gameObject);

        blocks.Clear();
    }

    private void UpdateBlocks()
    {
        for (int i = 0; i < blocks.Count; )
        {
            if (blocks[i].GO == null)
                blocks.RemoveAt(i);
            else
                i++;
        }
    }

    private void ActivateBuildingTool()
    {
        Tools.current = Tool.Move;
        Tools.hidden = true;
        SceneView.duringSceneGui += UpdateBuildingTool;
    }
    private void HideBuildingTool()
    {
        isBuilding = false;
        SceneView.duringSceneGui -= UpdateBuildingTool;
        Tools.hidden = false;
        DestroyImmediate(exampleGO);
    }

    private void UpdateBuildingTool(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(0);
        FocusWindowIfItsOpen(typeof(MapBuilderWindow));

        if (selectedBlock == null || selectedBlock.Sprite == null)
            return;

        Vector2 size = CalculateSize(selectedBlock.Sprite, gridSize);

        if (exampleGO != null)
            DestroyImmediate(exampleGO);
        exampleGO = new GameObject("block", typeof(SpriteRenderer));

        var eGoSprR = exampleGO.GetComponent<SpriteRenderer>();
        exampleGO.transform.localScale = size;
        eGoSprR.sprite = selectedBlock.Sprite;
        eGoSprR.color = new Color(1, 1, 1, 0.3f);
        exampleGO.hideFlags = HideFlags.HideAndDontSave;

        Vector2 bpos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        bpos = VectorUtility.Vec2ToVec2Int(bpos, gridSize);

        exampleGO.transform.position = (Vector3)bpos + Vector3.back;

        if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && 
            Event.current.button == 0)
        {
            var _pickedGO = HandleUtility.PickGameObject(Event.current.mousePosition, false, 
                (from o in GameObject.FindObjectsOfType(typeof(RectTransform)) select ((RectTransform)o).gameObject)
                .ToArray());

            if (_pickedGO == null || _pickedGO.GetComponent<SpriteRenderer>() == null ||
            _pickedGO.GetComponent<SpriteRenderer>().sortingLayerName != selectedBlock.SortingLayer)
            {
                if (_pickedGO != null)
                    Debug.Log(_pickedGO.GetComponent<SpriteRenderer>());
                var go = new GameObject(selectedBlock.Sprite.name);
                var _sprRend = go.AddComponent<SpriteRenderer>();

                Undo.RegisterCreatedObjectUndo(go, $"GO {go.name} created");

                blocks.Add(new Block(go, selectedBlock.Type));
                //go.GetComponent<SpriteRenderer>().sortingLayerName = orderLayer;
                //go.GetComponent<SpriteRenderer>().sortingOrder = order;

                go.transform.SetParent(folder);
                _sprRend.sprite = selectedBlock.Sprite;
                _sprRend.sortingLayerName = selectedBlock.SortingLayer;
                _sprRend.sortingOrder = selectedBlock.SortingOrder;

                if (setCollider)
                {
                    go.AddComponent(typeof(BoxCollider2D));
                    go.GetComponent<BoxCollider2D>().size = CalculateBoxColliderSize(selectedBlock.Sprite);
                }

                go.transform.localScale = size;
                go.transform.position = bpos;
            }
        }

        sceneView.Repaint();
    }

    public static Vector2 CalculateBoxColliderSize(Sprite spr)
    {
        return new Vector2(spr.rect.width / spr.pixelsPerUnit, spr.rect.height / spr.pixelsPerUnit);
    }

    public static Vector2 CalculateSize(Sprite spr, float gridSize)
    {
        return new Vector2(gridSize / (spr.rect.width / spr.pixelsPerUnit), gridSize / (spr.rect.height / spr.pixelsPerUnit));
    }

    [System.Serializable]
    private class InspectorBlock
    {
        public InspectorBlock() : this(null, "") { }
        public InspectorBlock(Sprite sprite, string type, string sortingLayer = "Default", int sortingOrder = 0)
        {
            this.sprite = sprite;
            this.type = type;
            this.sortingLayer = sortingLayer;
            this.sortingOrder = sortingOrder;
        }

        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private string type;
        [SerializeField]
        private string sortingLayer;
        [SerializeField]
        private int sortingOrder;

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }
        public string Type
        {
            get => type;
            set => type = value;
        }

        public string SortingLayer
        {
            get => sortingLayer;
            set => sortingLayer = value;
        }

        public int SortingOrder
        {
            get => sortingOrder;
            set => sortingOrder = value;
        }
    }



    [Serializable]
    private class Block
    {
        public Block(GameObject go, string type)
        {
            this.go = go;
            this.type = type;
        }
        [SerializeField]
        private GameObject go;
        [SerializeField]
        private string type;

        public GameObject GO => go;
        public string Type => type;
    }

    [Serializable]
    private class BlockSL
    {
        public BlockSL(Block b)
        {
            var _sprR = b.GO.GetComponent<SpriteRenderer>();

            type = b.Type;
            sprName = _sprR.sprite.name;
            sortingLayer = _sprR.sortingLayerName;
            sortingOrder = _sprR.sortingOrder;

            hasCollider = b.GO.GetComponent<BoxCollider2D>() != null;
            position = (Vector2)b.GO.transform.position;
            
        }

        private string type;
        private string sprName;
        private bool hasCollider;
        private string sortingLayer;
        private int sortingOrder;
        private Vector2 position;

        public Block GetBlock(Texture2D texAtlass, float gridSize)
        {
            GameObject go = new GameObject(sprName);
            var _sprR = go.AddComponent<SpriteRenderer>();
            go.transform.position = position;
            go.transform.SetParent(GameObject.Find(folderName).transform);

            _sprR.sprite = (Sprite)(AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(texAtlass)).First(o => o.name == sprName));
            _sprR.sortingLayerName = sortingLayer != null ? sortingLayer : "Default";
            _sprR.sortingOrder = sortingOrder;

            if (hasCollider)
            {
                go.AddComponent<BoxCollider2D>().size = MapBuilderWindow.CalculateBoxColliderSize(go.GetComponent<SpriteRenderer>().sprite);
            }

            go.transform.localScale = CalculateSize(go.GetComponent<SpriteRenderer>().sprite, gridSize);

            return new Block(go, type);
        }
    }

    [Serializable]
    private class InspectorBlockSL
    {
        public InspectorBlockSL(InspectorBlock inspBlock)
        {
            type = inspBlock.Type;
            spriteName = inspBlock.Sprite.name;
            sortingLayer = inspBlock.SortingLayer;
            sortingOrder = inspBlock.SortingOrder;
        }

        private string type;
        private string spriteName;
        private string sortingLayer;
        private int sortingOrder;

        public InspectorBlock GetBlock(Texture2D textureAtlas)
        {
            return new InspectorBlock((Sprite)(AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(textureAtlas)).First(o => o.name == spriteName)), type,
                sortingLayer != null ? sortingLayer : "Default", sortingOrder);
        }
    }

    [Serializable]
    private class MapBuilderWindowSL
    {
        public MapBuilderWindowSL(List<InspectorBlock> inspBlocks, List<Block> blocks, int gridSize)
        {
            this.inspBlocks = (from b in inspBlocks
                          select new InspectorBlockSL(b))
                          .ToList();
            this.blocks = (from b in blocks
                           select new BlockSL(b))
                           .ToList();
            this.gridSize = gridSize;
        }

        private List<InspectorBlockSL> inspBlocks;
        private List<BlockSL> blocks;
        private int gridSize;

        public (List<InspectorBlock> InspectorBlocks, List<Block> Blocks, int GridSize) GetMapBuilderWindowParameters(Texture2D textureAtlas)
        {
            return ((from b in inspBlocks select b.GetBlock(textureAtlas)).ToList(), (from b in blocks select b.GetBlock(textureAtlas, gridSize)).ToList(), gridSize);
        }
    }
}
