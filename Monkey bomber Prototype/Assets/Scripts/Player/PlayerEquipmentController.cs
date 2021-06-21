using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerEquipmentController : MonoBehaviour
{
    [SerializeField] private ItemsCollector _playerItemsCollector;
    [SerializeField] private WeaponIndicatorController _weaponIndicatorController;
    
    private List<ItemData> _equippableItems;
    private ItemData _equippedItem;

    public ItemData EquippedItem => _equippedItem;
    
    private void Awake()
    {
        _equippableItems = new List<ItemData>();
        
        _playerItemsCollector.onContainedItemsChanged += OnPlayerContainedItemsChanged;
    }
    
    private void Update()
    {   
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Previous();
        }
    }
    
    private void OnPlayerContainedItemsChanged()
    {
        RefreshAll();
    }

    private void RefreshItems()
    {
        _equippableItems = _playerItemsCollector.GetItems(i => i.IsEquippable).ToList();

        _equippedItem = _equippableItems.Find(i => i == _equippedItem);
    }

    private void RefreshAll()
    {
        RefreshItems();

        _weaponIndicatorController.RefreshIndicator(_equippedItem, _playerItemsCollector.GetCount(_equippedItem));
    }
    
    public void Next()
    {
        MoveAt(1);
    }

    public void Previous()
    {
        MoveAt(-1);
    }
    
    private void MoveAt(int i)
    {
        if (i == 0)
            return;

        int equippedItemIndex = _equippableItems.IndexOf(_equippedItem);
        
        if (equippedItemIndex != -1 || i > 0)
        {
            equippedItemIndex = i + equippedItemIndex;
        }
        else
        {
            equippedItemIndex = i + _equippableItems.Count;
        }

        if (equippedItemIndex >= _equippableItems.Count || equippedItemIndex < -1)
            equippedItemIndex = -1;

        _equippedItem = equippedItemIndex == -1 ? null : _equippableItems[equippedItemIndex];

        _weaponIndicatorController.RefreshIndicator(_equippedItem, _playerItemsCollector.GetCount(_equippedItem));
    }

    public void Remove(ItemData itemData)
    {
        _playerItemsCollector.RemoveOne(itemData);
    }
}
