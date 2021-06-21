using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using static Custom.Math.Math;

[CreateAssetMenu(fileName = "Grenade.asset", menuName = "Data/Item/Attack/Grenade")]
public class GrenadeData : ScriptableObject
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _speed;
    [SerializeField] private ParticleSystem _explosionVFX;
    [SerializeField] private float _damage;

    private float _maxDistance = -1;
    
    public float ExplosionRadius => _explosionRadius;
    public float Speed => _speed;
    public float Damage => _damage;
    public ParticleSystem ExplosionVFX => _explosionVFX;

    public float MaxDistance
    {
        get
        {
            if (_maxDistance < 0)
            {
                CalculateMaxDistance(0);
            }

            return _maxDistance;
        }
    }

    public float CalculateMaxDistance(float height)
    {
        float speedSqr = _speed * _speed;

        _maxDistance = SIN_45 * (speedSqr * SIN_45 +
                         _speed * Mathf.Sqrt(speedSqr * SIN_45 * SIN_45 - 2 * Physics.gravity.y * height)) / -Physics.gravity.y;

        return _maxDistance;
    }

    public bool TryGetUpperAngle(float distance, float height, out float angle)
    {
        angle = 0;

        if (distance > MaxDistance)
            return false;
        
        float sqrDistance = distance * distance;
        float sqrSpeed = _speed * _speed;
        float g = -Physics.gravity.y;
        float a = 4 * height * height * sqrSpeed * sqrSpeed;
        float b = 4 * sqrSpeed * sqrDistance * (sqrSpeed - g * height);
        float c = sqrDistance * (g * g * sqrDistance - 4 * sqrSpeed * sqrSpeed);

        float t = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / 2 / a;
        
        angle = -(Mathf.Acos(2 * t - 1) / 2f) * Mathf.Rad2Deg;
        
        return true; 
    }

    public bool TryCalculateSpeed(float distance, float height, Vector3 forward, out Vector3 speed)
    {
        speed = Vector3.zero;

        if (!TryGetUpperAngle(distance, height, out var angle))
            return false;

        speed = CalculateSpeed(Quaternion.Euler(angle, 0, 0) * Quaternion.LookRotation(forward));
        
        return true;
    }

    public Vector3 CalculateSpeed(Quaternion rotation)
    {
        return rotation *
               Vector3.forward *
               _speed;
    }
}
