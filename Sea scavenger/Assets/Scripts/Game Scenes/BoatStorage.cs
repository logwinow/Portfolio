using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Custom.Patterns;

public class BoatStorage : ItemsContainer
{
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private StorageItem storageItemPrefab;
    private StuffObject _stuffObject;
    
    private List<StorageItem> items;

    private void Awake()
    {
        items = new List<StorageItem>();
        _stuffObject = GetComponent<StuffObject>();

        GameSaver.onLoadAfterCheck += OnLoad;
        GameSaver.onSave += OnSave;
    }

    private void OnDestroy()
    {
        GameSaver.onLoadAfterCheck -= OnLoad;
        GameSaver.onSave -= OnSave;
    }

    private void Update()
    {
        if (SceneManager.Instance.PauseMenuController.IsPaused || 
            (Input.GetKeyDown(KeyCode.Tab) && scrollRect.gameObject.activeSelf))
        {
            _stuffObject.Close();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !scrollRect.gameObject.activeSelf && 
                 !SceneManager.Instance.IsOpenedAnySubmenu)
        {
            _stuffObject.Open();
        }
    }

    public bool TryTake(System.Predicate<SectionID> predicate, out SectionID id, int count = 1)
    {
        id = default;
        
        foreach (var i in items)
        {
            if (predicate(i.ID))
            {
                if (!i.gameObject.activeSelf || i.Count <= 0)
                    continue;

                var lastCount = i.Count - count;
                
                if (lastCount == 0)
                {
                    i.gameObject.SetActive(false);
                }
                else if (lastCount < 0)
                    continue;
                
                
                id = i.ID;
                i.Count = lastCount;
                
                return true;
            }
        }

        return false;
    }

    public bool Contains(SectionID id, int count = 1)
    {
        var item = items.Find(sti => sti.ID == id);

        if (item is null || !item.gameObject.activeSelf)
            return false;
        
        return item.Count >= count;
    }

    public void Put(SectionID sectionID, int count = 1)
    {
        StorageItem stItem = items.Find(i => i.ID == sectionID);

        if (stItem is null)
        {
            CreateItem(sectionID, count);

            return;
        }
        
        if (!stItem.gameObject.activeSelf)
        {
            stItem.gameObject.SetActive(true);
        }

        stItem.Count += count;
    }

    protected override void Put(CollectableObject collectableObject)
    { 
        Put(collectableObject.ID);
    }

    protected override bool PutValidate()
    {
        return true;
    }

    private void CreateItem(SectionID sectionID, int count)
    {
        StorageItem _item;

        _item = Instantiate(storageItemPrefab, scrollRect.content);
        _item.Set(sectionID, count);
        items.Add(_item);
    }

    private void OnLoad()
    {
        GameSaver.SavedGlobalData.LoadToStorage(this);
    }

    private void OnSave()
    {
        foreach (var si in items)
        {
            GameSaver.SavedGlobalData.SetItem(si.ID, si.Count);
        }
    }
}
