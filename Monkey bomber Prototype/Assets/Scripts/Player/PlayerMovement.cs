using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private bool _canMoving = true; 
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_canMoving)
            return;
        
        _rb.velocity = transform.TransformVector(
            Vector3.ClampMagnitude(
                new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1
                ) * _speed
            );
    }

    public void OnAttackActivated()
    {
        _rb.velocity = Vector3.zero;
        
        _canMoving = false;
    }

    public void OnAttackDeactivated()
    {
        _canMoving = true;
    }
}
