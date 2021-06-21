using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, IReusable, ISpawnable
{
    [SerializeField] private ItemData _itemData;

    public ItemData ItemData => _itemData;
    
    private bool _isCollectibleActive;
    
    public event Action onDespawn;
    
    public void Active()
    {
        onDespawn = null;
        
        _isCollectibleActive = true;
    }

    private void OnCollect()
    {
        onDespawn?.Invoke();
        
        Deactive();
        
        ItemsManager.Instance.ReturnToPool(this);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!_isCollectibleActive || !other.TryGetComponent(out ItemsCollector itemsCollector)) 
            return;
        
        itemsCollector.Put(_itemData);
            
        OnCollect();
    }

    public void Deactive()
    {
        _isCollectibleActive = false;
    }

    void IReusable.Reuse()
    {
        Active();
    }
}
