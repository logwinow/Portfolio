using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointCollectible : SpawnPoint
{
    [SerializeField] private ItemData _spawnItemData;

    protected override void Set()
    {
        Set(ItemsManager.Instance.GetFromPool(_spawnItemData));
    }
}
