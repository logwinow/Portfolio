using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom.Utility;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Custom.Math.Math;

[RequireComponent(typeof(LineRenderer))]
public class PlayerProjectileAttackController : ProjectileAttackController
{
    [SerializeField] private PlayerEquipmentController _playerEquipmentController;
    [SerializeField] private float _trajectoryPrecision;
    [SerializeField] private PlayerRotation _playerRotation;
    [SerializeField] private float _angleDelta = 15f;
    [SerializeField] private CrossfireController _crossfireController;

    private LineRenderer _lineRenderer;
    private bool _isTrajectoryVisible;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetActiveTrajectory(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_playerEquipmentController.EquippedItem is AttackerItemData attackerItemData)
            {
                InitializeAttack(attackerItemData);
            }
        }

        if (attackerItemData == null)
            return;
        
        if (Input.GetMouseButtonUp(0))
        {
            Throw(CalculateStartSpeed());
            
            return;
        }
        
        ProcessAttack();
    }

    public override void InitializeAttack(AttackerItemData attackerItemData)
    {
        base.InitializeAttack(attackerItemData);
        
        _crossfireController.Show();
        SetActiveTrajectory(true);
    }

    private void ProcessAttack()
    {
        ShowTrajectory(shootPointTransform.position,
            RecalculateSpeed(CalculateStartSpeed())
            ); 
    }

    private Vector3 CalculateStartSpeed()
    {
        var angle = _playerRotation.Angle;
        angle.x = Mathf.Clamp(angle.x + _angleDelta, -90f, 90f);

        return attackerItemData.GrenadeAttackData.CalculateSpeed(Quaternion.Euler(angle));
    }

    protected override void EndAttack()
    {
        base.EndAttack();
        
        _crossfireController.Close();
        SetActiveTrajectory(false);
    }

    private void ShowTrajectory(Vector3 origin, Vector3 speed)
    {
        var points = new List<Vector3> { origin };
        float timeStep = 1 / _trajectoryPrecision;
        Vector3 currentPoint = origin;
        
        while  (currentPoint.y > 0)
        {
            currentPoint += speed * timeStep;
            speed += Physics.gravity * timeStep;
            
            points.Add(currentPoint);
        }

        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
        
        if (!_isTrajectoryVisible)
            SetActiveTrajectory(true);
    }

    private void SetActiveTrajectory(bool value)
    {
        _lineRenderer.enabled = value;

        _isTrajectoryVisible = value;
    }
}
