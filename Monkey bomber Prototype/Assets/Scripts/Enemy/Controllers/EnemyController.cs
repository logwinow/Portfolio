using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyController : MonoBehaviour, ISpawnable, IReusable
{
    [SerializeField] private EnemyFSM _fsm;
    
    private HealthController _enemyHealthController;

    public Transform Target
    {
        get => _fsm.Target;
        set => _fsm.Target = value;
    }
    
    public event Action onDespawn;

    private void Awake()
    {
        _enemyHealthController = GetComponent<HealthController>();
        _enemyHealthController.onDead += OnDead;

        _fsm.Initialize(
            new List<State>
            {
                new EnemyMoveState(_fsm), 
                new EnemyAttackState(_fsm)
            },
            "EnemyMove");
    }

    private void Update()
    {
        _fsm.Process();
    }

    private void OnDestroy()
    {
        _enemyHealthController.onDead -= OnDead;
    }

    private void OnDead()
    {        
        onDespawn?.Invoke();
        
        EnemiesManager.Instance.ReturnToPool(this);
    }

    public void Reuse()
    {
        _fsm.SetNextActionState("EnemyMove");
    }
}
