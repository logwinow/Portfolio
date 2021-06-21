using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item.asset", menuName = "Data/Item/Base")]
public class ItemData : ScriptableObject
{
    [SerializeField] private bool _isEquippable;
    [SerializeField] private bool _isStackable;
    [SerializeField] private Color _iconColor;

    public bool IsEquippable => _isEquippable;
    public bool IsStackable => _isStackable;
    public Color IconColor => _iconColor;
}
