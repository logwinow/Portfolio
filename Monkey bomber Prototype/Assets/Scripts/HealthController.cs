using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IReusable
{
    [SerializeField] private HealthData _healthData;

    private float _health;

    public event Action<float> onDamage;
    public event Action onDead;

    private void Awake()
    {
        _health = _healthData.Health;
    }

    public void Hit(float damage)
    {
        _health -= damage;
        
        onDamage?.Invoke(damage);

        if (_health <= 0)
        {
            onDead?.Invoke();
            
            Dead();
        }
    }

    protected virtual void Dead()
    {
        
    }

    public void Reuse()
    {
        _health = _healthData.Health;
    }
}
