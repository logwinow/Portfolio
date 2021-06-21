using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  TMPro;

public class DepthController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private TextMeshProUGUI _numberTMP;
    [SerializeField] private int _originDepth;
    [SerializeField] private float _multiplier;

    private float _offset;
    public int Depth { get; private set; }
    
    private void Start()
    {
        _offset = -_originDepth - _target.transform.position.y * _multiplier;
    }

    private void Update()
    {
        UpdateDepth();
    }

    private void SetNumber(float n)
    {
        _numberTMP.text = $"{Mathf.Abs((int)n)} m";
    }

    private int CalculateDepth()
    {
        return Depth = (int)Mathf.Abs(_target.transform.position.y * _multiplier + _offset);
    }

    private void UpdateDepth()
    {
        SetNumber(CalculateDepth());
    }
}
