using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Custom.SmartCoroutines;

public class CameraZooming : MonoBehaviour
{
    [SerializeField] private float _zoomingSpeed;
    [SerializeField] private float _zoomingTo;

    private float _prevZoom;
    private Camera _camMain;
    private SmartCoroutineCache<float> _zoomingCor;

    private void Awake()
    {
        _camMain = Camera.main;
        _zoomingCor = new SmartCoroutineCache<float>(this, ZoomingRoutine);

        _prevZoom = _camMain.orthographicSize;
    }

    public void StartZooming()
    {
        StartZooming(_zoomingTo);
    }
    
    public void StartZooming(float to)
    {
        _prevZoom = _camMain.orthographicSize;
        
        _zoomingCor.Start(to);
    }

    public void Stop()
    {
        _zoomingCor.Stop();
    }

    public void ReturnToPreviousZoom()
    {
        _zoomingCor.Start(_prevZoom);
    }

    private IEnumerator ZoomingRoutine(float to)
    {
        float _prevOrthSize;
        
        while (true)
        {
            _prevOrthSize = _camMain.orthographicSize;
            
            _camMain.orthographicSize = Mathf.Lerp(_camMain.orthographicSize, to, 
                Time.deltaTime * _zoomingSpeed);

            yield return null;
        }
    }
}
