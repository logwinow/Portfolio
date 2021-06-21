using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Extensions.Vector;
using  Custom.SmartCoroutines;

public class CameraSmoothFollow : MonoBehaviour
{
    private const float MIN_SQR_DIST = 0.01f;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float stiffness = 1f;

    private SmartCoroutineCache<Transform> _smoothingToTrCor;
    private SmartCoroutineCache<Vector3> _smoothingToPosCor;
    
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    
    private void Awake()
    {
        _smoothingToTrCor = new SmartCoroutineCache<Transform>(this, SmoothingToTransformRoutine);
        _smoothingToPosCor = new SmartCoroutineCache<Vector3>(this, SmoothingToPositionRoutine);
    }

    private void Start()
    { 
        StartSmoothing(target);
    }

    public void StartSmoothing(Transform smoothToTransform)
    {
        StopSmoothing();
        _smoothingToTrCor.Start(smoothToTransform);
    }
    
    public void StartSmoothing(Vector3 smoothToPosition)
    {
        StopSmoothing();
        _smoothingToPosCor.Start(smoothToPosition);
    }

    public void StopSmoothing()
    {
        _smoothingToTrCor.Stop();
        _smoothingToPosCor.Stop();
    }

    private void SmoothTo(Vector3 toPos)
    {
        transform.position = Vector3.Lerp(transform.position, toPos, stiffness * Time.deltaTime)
            .SetComponentValue(VectorUtility.ComponentType.Z, transform.position.z);
    }

    private IEnumerator SmoothingToTransformRoutine(Transform toTr)
    {
        while (true)
        {
            SmoothTo(toTr.position);
            
            yield return null;
        }
    }
    
    private IEnumerator SmoothingToPositionRoutine(Vector3 toPos)
    {
        Vector3 prevPos;
        
        while (true)
        {
            prevPos = transform.position;
            
            SmoothTo(toPos);
            
            if ((prevPos - transform.position).sqrMagnitude < 0.00001f)
                break;
            
            yield return null;
        }
    }
}
