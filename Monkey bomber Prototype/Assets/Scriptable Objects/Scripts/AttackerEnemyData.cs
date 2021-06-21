using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackerEnemyBase.asset", menuName = "Data/Stats/Enemy/Attacker/Base")]
public class AttackerEnemyData : HealthData
{
    [SerializeField] private float _attackDelay;
    [SerializeField] private AttackerItemData _attackerItemData;

    public float AttackDelay => _attackDelay;
    public AttackerItemData AttackerItemData => _attackerItemData;
}
