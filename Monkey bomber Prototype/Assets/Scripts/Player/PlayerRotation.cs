using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField]
    private Transform _playerCameraTransform;
    [SerializeField]
    private float _maxXRot = 90f;
    [SerializeField]
    private float _minXRot = -90f;
    [SerializeField]
    private float _sensitive = 1f;

    private Vector3 curRot;

    public Vector3 Angle => curRot;

    private void Start()
    {
        curRot = transform.localEulerAngles;
    }

    private void Update()
    {
        float rotX = -Input.GetAxis("Mouse Y") * _sensitive; 
        float rotY = Input.GetAxis("Mouse X") * _sensitive; 

        curRot.x = Mathf.Clamp(curRot.x + rotX, _minXRot, _maxXRot);
        curRot.y = Mathf.Repeat(curRot.y + rotY, 360f);

        transform.rotation = Quaternion.Euler(0, curRot.y, 0);
        _playerCameraTransform.localRotation = Quaternion.Euler(curRot.x, 0, 0);
    }
}
