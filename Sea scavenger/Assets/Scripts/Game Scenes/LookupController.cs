using System;
using System.Collections;
using System.Collections.Generic;
using Custom.Extensions.Vector;
using UnityEngine;
using UnityEngine.UI;

// Событие OnPlayerDead
// Если ни одно из меню не открыто, то активируется
public class LookupController : MonoBehaviour
{
    [SerializeField] private float _lookupDistance;
    [SerializeField] private CameraShift _cameraShift;
    [SerializeField] private float _toZoom;
    [SerializeField] private int _sceneBuildIndex;

    private Transform _lookupTransf;

    private void Awake()
    {
        _lookupTransf = new GameObject().transform;
        
        UnityEngine.SceneManagement.SceneManager
            .MoveGameObjectToScene(_lookupTransf.gameObject, 
                UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(_sceneBuildIndex));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _lookupTransf.position =
                VectorUtility.DirectionVectorWithoutComponent(
                    PlayerController.Instance.CameraTarget.transform.position,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    VectorUtility.ComponentType.Z) * _lookupDistance
                + PlayerController.Instance.CameraTarget.transform.position;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift) && !SceneManager.Instance.IsOpenedAnySubmenu)
        {
            _cameraShift.ReturnToDefault();
            return;
        }
        
        if (!Input.GetKeyDown(KeyCode.LeftShift))
            return;

        if (SceneManager.Instance.IsOpenedAnySubmenu ||
            SceneManager.Instance.PauseMenuController.IsPaused
            || PlayerController.Instance.IsDead)
            return;
        
        _cameraShift.Shift(_lookupTransf, _toZoom);
    }
}
