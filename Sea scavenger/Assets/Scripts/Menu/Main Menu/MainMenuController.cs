using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _warningPanel;

    private void Start()
    {
        _warningPanel.gameObject.SetActive(false);
    }

    public void FadeIn()
    {   
        _animator.SetBool("IsVisible", true);
        _canvasGroup.interactable = true;
        
        _continueButton.SetActive(ContinueButtonValidate());
    }

    public void TryNewGameButton()
    {
        if (ContinueButtonValidate())
        {    
            _warningPanel.gameObject.SetActive(true);
        }
        else
        {
            ForceNewGameButton();
        }
    }

    public void ContinueButton()
    {
        GameManager.Instance.GoToScene(1, false);
    }
    
    public void ForceNewGameButton()
    {
        GameSaver.DeleteAllSaves();
        GameManager.Instance.GoToScene(1, false);
    }

    public void CloseWarningPanelButton()
    {
        _warningPanel.SetActive(false);
    }
    
    public void ExitButton()
    {
        print("fuck");
        Application.Quit();
    }

    private bool ContinueButtonValidate()
    {
        return GameSaver.CheckSaves();
    }
}
