using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom.Patterns;
using UnityEngine;

public class EnemiesManager : Singleton<EnemiesManager>
{
    
    [SerializeField] private EnemyController _enemiyPrefab;
    [SerializeField] private Transform _target;
    
    private Pool<EnemyController> _enemiesPool;

    protected override void Init()
    {
        _enemiesPool = new Pool<EnemyController>(transform, _enemiyPrefab, 
            activateAction: delegate(EnemyController enemy)
            {
                enemy.gameObject.SetActive(true);
                
                foreach (var c in enemy.GetComponents<IReusable>())
                {
                    c.Reuse();
                }

                enemy.Target = _target;
            });
    }

    public EnemyController GetFromPool()
    {
        return _enemiesPool.GetAvailable();
    }

    public void ReturnToPool(EnemyController enemy)
    {   
        _enemiesPool.Release(enemy);
    }
}
