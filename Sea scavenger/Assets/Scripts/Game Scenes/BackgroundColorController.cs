using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorController : MonoBehaviour
{
    [SerializeField] private DepthController _depthController;
    [SerializeField] private Material _bgMaterial;
    [SerializeField] private float _exposureMax = 1.64f;
    [SerializeField] private float _exposureMin = 0.3f;
    [SerializeField] private int maxDepth = 300;

    private void Update()
    {
        _bgMaterial.SetFloat("_Exposure", 
            Mathf.Lerp(_exposureMax, _exposureMin, 
                Mathf.Clamp01((float)_depthController.Depth / maxDepth)));
    }
}
