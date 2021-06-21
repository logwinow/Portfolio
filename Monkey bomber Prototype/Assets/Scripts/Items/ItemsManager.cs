using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Custom.Patterns;

public class ItemsManager : Singleton<ItemsManager>
{
    [SerializeField] private Collectible[] _itemsPrefabs;
    
    private Pool<Collectible> _itemsPool;

    protected override void Init()
    {
        _itemsPool = new Pool<Collectible>(transform, null, 
            activateAction: delegate(Collectible collectible)
            {
                collectible.gameObject.SetActive(true);
                foreach (var c in collectible.GetComponents<IReusable>())
                {
                    c.Reuse();
                }
            });
    }

    public Collectible GetFromPool(ItemData itemData)
    {
        return _itemsPool.GetAvailableOrNull(c => !c.gameObject.activeSelf && c.ItemData == itemData) ?? 
            _itemsPool.CreateNew(_itemsPrefabs.First(c => c.ItemData == itemData));
    }

    public void ReturnToPool(Collectible collectible)
    {   
        _itemsPool.Release(collectible);
    }
}
