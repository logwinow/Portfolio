using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class GrenadeProjectile : MonoBehaviour, IReusable
{
    [SerializeField] private GrenadeData _grenadeData;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    private ParticleSystem _explosionVFX;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Throw(Vector3 speed)
    {
        Activate(true);
        _rigidbody.AddForce(speed, ForceMode.VelocityChange);
        _rigidbody.AddTorque(speed);
    }

    private void Activate(bool value)
    {
        _rigidbody.isKinematic = !value;
        _collider.isTrigger = !value;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_explosionVFX == null)
        {
            _explosionVFX = Instantiate(_grenadeData.ExplosionVFX);
        }

        _explosionVFX.transform.position = transform.position;
        _explosionVFX.Play();

        var hitColliders = Physics.OverlapSphere(transform.position, _grenadeData.ExplosionRadius);
        HealthController hitColliderHealthController;

        foreach (var c in hitColliders)
        {
            if ((hitColliderHealthController = c.GetComponent<HealthController>()) != null)
            {
                hitColliderHealthController.Hit(_grenadeData.Damage);
            }
        }

        ItemsManager.Instance.ReturnToPool(GetComponent<Collectible>());
    }
    
    void IReusable.Reuse()
    {
        Activate(false);
        
        transform.rotation = Quaternion.identity;
    }
}
