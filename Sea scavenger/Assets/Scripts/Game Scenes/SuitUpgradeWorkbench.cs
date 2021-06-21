using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  System;
using System.Reflection;
using TMPro;

public class SuitUpgradeWorkbench : StuffObject
{
    [SerializeField] private SuitController[] suits;
    [SerializeField] private CameraShift _cameraShift;
    [SerializeField] private float _shift;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;
    [SerializeField] private TextMeshProUGUI suitTitleTMP;
    [SerializeField] private SectionID _thirdSuitPartsParameterID;

    private int currentSuitIndex = 0;

    private void Start()
    {
        var parameterValue = GameManager.Instance.GetParameter(_thirdSuitPartsParameterID).Value;

        switch (parameterValue)
        {
            case 0:
                suits[1].Disable();
                break;
            case int i when i < 4:
                suits[1].UpdateSprite();
                suits[1].IsAvailable = false;
                break;
            default:
                suits[1].UpdateSprite();
                break;
        }
        
        DisableArrows();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) 
            || SceneManager.Instance.PauseMenuController.IsPaused)
        {
            Close();
        }
    }

    public override void Close()
    {
        _scrollRect.gameObject.SetActive(false);
        DisableArrows();
        ContextMenu.Instance.Close();
        
        _cameraShift.ReturnToDefault();
        PlayerController.Instance.EnableMovement();
        PlayerController.Instance.EnableInteractions();
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsInteracting", false);
        
        SceneManager.Instance.RemoveActiveSubmenu(this);
    }

    public override void Open()
    {
        _scrollRect.gameObject.SetActive(true);
        UpdateArrows();

        GoTo(0);
        
        PlayerController.Instance.DisableMovement();
        PlayerController.Instance.DisableInteractions();
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsInteracting", true);
        
        SceneManager.Instance.AddActiveSubmenu(this);
    }

    public void GoNext()
    {
        GoTo(currentSuitIndex + 1);
    }

    public void GoBack()
    {
        GoTo(currentSuitIndex - 1);   
    }

    private void GoTo(int index)
    {
        suits[currentSuitIndex].CloseUpgrades();
        
        currentSuitIndex = index;
        
        suitTitleTMP.text = suits[currentSuitIndex].Title;
        suits[currentSuitIndex].FocusCamera(_cameraShift, _shift);
        suits[currentSuitIndex].OutputUpgrades();
        
        UpdateArrows();
    }

    private void UpdateArrows()
    {
        _leftArrow.SetActive(ValidateLeftArrow());
        _rightArrow.SetActive(ValidateRightArrow());
    }

    private void DisableArrows()
    {
        _leftArrow.SetActive(false);
        _rightArrow.SetActive(false);
    }

    private bool ValidateRightArrow()
    {
        if (currentSuitIndex == suits.Length - 1
        || !suits[currentSuitIndex + 1].IsAvailable)
            return false;
        
        return true;
    }

    private bool ValidateLeftArrow()
    {
        if (currentSuitIndex == 0)
            return false;

        return true;
    }
}
