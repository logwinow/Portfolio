using System;
using System.Collections;
using System.Collections.Generic;
using Custom.Extensions.UI;
using UnityEngine;
using Custom.SmartCoroutines;
using UnityEngine.UI;
using  Custom.Extensions.Vector;

public class CameraShift : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _toZoom;
    [SerializeField] private CameraZooming _zooming;
    [SerializeField] private CameraSmoothFollow _smoothFollow;

    private SmartCoroutineCache<Vector3, float> _shiftingPositionCor;
    private SmartCoroutineCache<Transform, float> _shiftingTransformCor;

    private void Awake()
    {
        _shiftingPositionCor = new SmartCoroutineCache<Vector3, float>(this, ShiftingRoutine);
        _shiftingTransformCor = 
            new SmartCoroutineCache<Transform, float>(this, ShiftingRoutine);
    }

    public void Shift(Vector3 targetPosition)
    {
        Shift(targetPosition, _toZoom);
    }

    public void Shift(Vector3 targetPosition, float zoom)
    {
        StopShifting();
        
        _zooming.Stop();
        _smoothFollow.StopSmoothing();
        
        _shiftingPositionCor.Start(targetPosition, zoom);
    }

    public void Shift(Transform target, float zoom)
    {
        StopShifting();
        
        _zooming.Stop();
        _smoothFollow.StopSmoothing();

        _shiftingTransformCor.Start(target, zoom);
    }

    public void Shift(Transform target)
    {
        Shift(target, _toZoom);
    }

    public void ReturnToDefault()
    {
        StopShifting();
        
        _zooming.ReturnToPreviousZoom();
        _smoothFollow.StartSmoothing(_smoothFollow.Target);
    }

    private void StopShifting()
    {
        _shiftingPositionCor.Stop();
        _shiftingTransformCor.Stop();
    }

    private IEnumerator ShiftingRoutine(Vector3 toPos, float zoom)
    {
        float curDist;
        float startDist = VectorUtility.DistanceWithoutComponent(transform.position, toPos, 
            VectorUtility.ComponentType.Z);
        float deltaZoom = Mathf.Abs(Camera.main.orthographicSize - zoom);
        
        do
        {
            transform.position = Vector3.Lerp(transform.position, toPos, Time.deltaTime * _speed)
                .SetComponentValue(VectorUtility.ComponentType.Z, transform.position.z);

            curDist = VectorUtility.DistanceWithoutComponent(transform.position, toPos, 
                VectorUtility.ComponentType.Z);
            
            Camera.main.orthographicSize = curDist / startDist
                                           * deltaZoom
                                           + zoom;

            yield return null;
        } while (curDist > 0.01f);
    }
    
    private IEnumerator ShiftingRoutine(Transform target, float zoom)
    {
        while(true)
        {
            transform.position = Vector3.Lerp(
                    transform.position, 
                    target.transform.position,
                    Time.deltaTime * _speed)
                .SetComponentValue(VectorUtility.ComponentType.Z, transform.position.z);

            Camera.main.orthographicSize = Mathf.Lerp(
                Camera.main.orthographicSize,
                zoom,
                Time.deltaTime * _speed);

            yield return null;
        }
    }
}
