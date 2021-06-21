using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemsCollector : MonoBehaviour
{
    private Dictionary<ItemData, int> _containedItems;

    public event Action onContainedItemsChanged; 

    private void Awake()
    {
        _containedItems = new Dictionary<ItemData, int>();
    }

    public void Put(ItemData itemData)
    {
        if (_containedItems.ContainsKey(itemData))
        {
            _containedItems[itemData]++;
        }
        else
            _containedItems[itemData] = 1;
        
        onContainedItemsChanged?.Invoke();
    }

    private void RemoveCount(ItemData itemData, int count, bool canRemoveMoreThanHad = false)
    {
        if (GetCount(itemData) == 0)
            return;

        if (!canRemoveMoreThanHad && _containedItems[itemData] - count < 0)
            return;
        
        if ((_containedItems[itemData] -= count) <= 0)
            _containedItems.Remove(itemData);
        
        onContainedItemsChanged?.Invoke();
    }
    
    public void RemoveOne(ItemData itemData)
    {
        RemoveCount(itemData, 1);
    }

    public void RemoveAll(ItemData itemData)
    {
        RemoveCount(itemData, int.MaxValue, true);
    }

    public int GetCount(ItemData itemData)
    {
        foreach (var i in _containedItems)
        {
            if (i.Key == itemData)
            {
                return i.Value;
            }
        }

        return 0;
    }

    public IEnumerable<ItemData> GetItems(Predicate<ItemData> predicate)
    {
        return from kvp in _containedItems
            where predicate(kvp.Key)
            select kvp.Key;
    }
}
