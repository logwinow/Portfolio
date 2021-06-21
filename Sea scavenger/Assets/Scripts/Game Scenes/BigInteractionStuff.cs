using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigInteractionStuff : StuffObject
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private CameraShift _cameraShift;
    [SerializeField] private float _shift;
    
    private Vector3 center;

    private void Awake()
    {
        center = GetComponent<SpriteRenderer>().bounds.center;
    }

    public override void Close()
    {
        scrollRect.gameObject.SetActive(false);
        
        PlayerController.Instance.EnableMovement();
        PlayerController.Instance.EnableInteractions();
        _cameraShift.ReturnToDefault();
        PlayerController.Instance.AnimationController.DBAnimationController.
            SetBool("IsInteracting", false);
        
        SceneManager.Instance.RemoveActiveSubmenu(this);
    }

    public override void Open()
    {
        scrollRect.gameObject.SetActive(true);
        
        PlayerController.Instance.DisableMovement();
        PlayerController.Instance.DisableInteractions();
        _cameraShift.Shift(center + Vector3.right * _shift);
        PlayerController.Instance.AnimationController.DBAnimationController.
            SetBool("IsInteracting", true);
        
        SceneManager.Instance.AddActiveSubmenu(this);
    }
}
