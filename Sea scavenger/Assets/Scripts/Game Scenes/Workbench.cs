using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor;

public class Workbench : StuffObject, ISavable
{
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField] private SectionID[] upgradesIds;
    [SerializeField] private CameraShift _cameraShift;
    [SerializeField] private float _shift;

    private Vector3 center;
    private bool _initialized;

    private void Awake()
    {
        center = GetComponent<SpriteRenderer>().bounds.center;
    }

    private void Start()
    {
        if (!_initialized)
            CreateItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) 
            || SceneManager.Instance.PauseMenuController.IsPaused)
        {
            Close();
        }
    }

    private void CreateItems(List<WorkbenchItemSL> loadedItems = null)
    {
        for (int i = 0; i < upgradesIds.Length; i++)
        {
            Instantiate(
                GameManager.Instance.WorkbenchItemPrefab, scrollRect.content)
                .Set(upgradesIds[i], loadedItems?[i].CurrentUpgrade ?? 0);
        }

        _initialized = true;
    }

    public override void Close()
    {
        if (!scrollRect.gameObject.activeSelf)
            return;
        
        scrollRect.gameObject.SetActive(false);

        ContextMenu.Instance.Close();
        
        PlayerController.Instance.EnableMovement();
        PlayerController.Instance.EnableInteractions();
        _cameraShift.ReturnToDefault();
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsInteracting", false);
        
        SceneManager.Instance.RemoveActiveSubmenu(this);
    }

    public override void Open()
    {
        scrollRect.gameObject.SetActive(true);
        
        PlayerController.Instance.DisableMovement();
        PlayerController.Instance.DisableInteractions();
        _cameraShift.Shift(center + Vector3.right * _shift);
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsInteracting", true);
        
        SceneManager.Instance.AddActiveSubmenu(this);
    }

    public void Load(object obj)
    {
        var wb = obj as WorkbenchSL;

        if (_initialized)
        {
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                Destroy(scrollRect.content.GetChild(i).gameObject);
            }
        }
        
        CreateItems(wb!.WorkbenchItems);
    }

    public bool Save(out object obj)
    {
        obj = new WorkbenchSL(this);

        return true;
    }

    [Serializable]
    private class WorkbenchSL
    {
        public WorkbenchSL(Workbench wb)
        {
            _workbenchItems = new List<WorkbenchItemSL>();
            
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var content = (typeof(Workbench)
                    .GetField("scrollRect", flags)!
                .GetValue(wb) as ScrollRect)!.content;

            for (int i = 0; i < content.childCount; i++)
            {
                _workbenchItems.Add(new WorkbenchItemSL(
                    content.GetChild(i).gameObject
                        .GetComponent<WorkbenchItem>()));
            }
        }

        private List<WorkbenchItemSL> _workbenchItems;

        public List<WorkbenchItemSL> WorkbenchItems => _workbenchItems;
    }
    
    [Serializable]
    private class WorkbenchItemSL
    {
        public WorkbenchItemSL(WorkbenchItem wi)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var cUpg = (typeof(WorkbenchItem)
                        .GetField("currentUpgradeIcon", flags)!
                    .GetValue(wi)
                as UpgradeIcon);

            if (cUpg == null)
            {
                _currentUpgrade = -1;
            }
            else
            {
                _currentUpgrade = ((List<UpgradeIcon>) typeof(WorkbenchItem)
                            .GetField("upgradeIcons", flags)!
                        .GetValue(wi))
                    .IndexOf(cUpg);
            }
        }
            
        private int _currentUpgrade;
        public int CurrentUpgrade => _currentUpgrade;
    }
}
