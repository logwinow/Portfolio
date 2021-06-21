using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Custom.Patterns;

public class Inventory : ItemsContainer
{
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private InventoryItem inventoryItemTemplatePrefab;
    [ReadOnly]
    [SerializeField]
    private int maxCountOfItems = 8;
    [SerializeField] private CameraShift _cameraShift;
    [SerializeField] private SectionID _inventoryCapacityParameterID;

    private Dictionary<CollectableObject, GameObject> items; // k: scene's go, v: ui go
    private Pool<InventoryItem> poolUIOfItems;

    private void Awake()
    {
        items = new Dictionary<CollectableObject, GameObject>();
        poolUIOfItems = new Pool<InventoryItem>(scrollRect.content, inventoryItemTemplatePrefab, 
            i => i.IsAvailable, delegate(InventoryItem item)
            {
                item.IsAvailable = false;
                item.SetImageActive(true);
            }, delegate(InventoryItem item)
            {
                item.IsAvailable = true;
                item.SetImageActive(false);
            });
        
        GameSaver.onSave += OnSave;
        GameSaver.onLoadAfterCheck += OnLoad;
    }

    private void OnDestroy()
    {
        GameSaver.onSave -= OnSave;
        GameSaver.onLoadAfterCheck -= OnLoad;
    }

    private void Update()
    {
        if (PlayerController.Instance.IsDead 
            || SceneManager.Instance.PauseMenuController.IsPaused)
        {
            if (scrollRect.gameObject.activeSelf)
            {
                Close();
            }
            
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (scrollRect.gameObject.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    private void Open()
    {
        _cameraShift.Shift(PlayerController.Instance.CameraTarget);
        scrollRect.gameObject.SetActive(true);
        PlayerController.Instance.DisableInteractions();
        PlayerController.Instance.DisableMovement();
        
        SceneManager.Instance.AddActiveSubmenu(this);
    }

    private void Close()
    {
        _cameraShift.ReturnToDefault();
        scrollRect.gameObject.SetActive(false);
        PlayerController.Instance.EnableInteractions();
        PlayerController.Instance.EnableMovement();
        
        SceneManager.Instance.RemoveActiveSubmenu(this);
    }

    protected override void Put(CollectableObject scitem)
    {
        var invItem = poolUIOfItems.GetAvailable();
        invItem.Set(scitem.GetComponent<SpriteRenderer>().sprite);

        items[scitem] = invItem.gameObject;
    }

    protected override bool PutValidate()
    {
        return items.Count < maxCountOfItems;
    }

    public void Remove(GameObject uiitem)
    {
        var item = items.First(kvp => kvp.Value == uiitem);

        items.Remove(item.Key);
        poolUIOfItems.Release(uiitem.GetComponent<InventoryItem>());

        PlayerController.Instance.InteractionController.CollectorController.Throw(item.Key);
    }

    public List<SectionID> Save()
    {
        return (from i in items select i.Key.ID).ToList();
    }

    private void OnSave()
    {
        foreach (var i in items)
        {
            GameSaver.SavedGlobalData.AddItemCount(i.Key.ID);
        }
    }

    private void OnLoad()
    {
        maxCountOfItems = 
            GameManager.Instance.GetParameter(_inventoryCapacityParameterID).Value;

        for (int i = 0; i < maxCountOfItems; i++)
        {
            poolUIOfItems.Release(poolUIOfItems.CreateNew());
        }
    }
}
