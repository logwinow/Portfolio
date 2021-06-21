using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health.asset", menuName = "Data/Stats/Base")]
public class HealthData : ScriptableObject
{
    [SerializeField] private float _health;

    public float Health => _health;
}
