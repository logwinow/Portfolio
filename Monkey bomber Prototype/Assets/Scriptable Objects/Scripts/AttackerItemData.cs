using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackerItem.asset", menuName = "Data/Item/AttackerItem")]
public class AttackerItemData : ItemData
{
    [SerializeField] private GrenadeData _grenadeAttackData;
    
    public GrenadeData GrenadeAttackData => _grenadeAttackData;
}
