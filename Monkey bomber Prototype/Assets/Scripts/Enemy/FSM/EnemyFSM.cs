using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemyFSM : FSM
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private AttackerEnemyData _attackerEnemyData;
    [SerializeField] private ProjectileAttackController _projectileAttackController;

    private GrenadeData _grenadeData;

    public Transform Target { get; set; }
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public AttackerEnemyData AttackerEnemyData => _attackerEnemyData;
    public ProjectileAttackController ProjectileAttackController => _projectileAttackController;

    public GrenadeData GrenadeData
    {
        get
        {
            if (_grenadeData == null)
                _grenadeData = _attackerEnemyData.AttackerItemData.GrenadeAttackData as GrenadeData;

            return _grenadeData;
        }
    }
}
