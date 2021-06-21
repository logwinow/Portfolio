using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;
using TMPro;

public class DivingMenuController : PauseMenuController
{
    [SerializeField] private float _delayAfterDeadTime;
    [SerializeField] private GameObject _loadLastSaveButton;
    [SerializeField] private GameObject _returnToBoatButton;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private TextMeshProUGUI _title;

    private SmartCoroutineCache _deadMenuCallCor;
    
    private void Awake()
    {
        callCondition = () => !PlayerController.Instance.IsDead;
        _deadMenuCallCor = new SmartCoroutineCache(this, DeadMenuDelayRoutine);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void ReturToBoatButton()
    {
        Time.timeScale = 1f;
        
        GameManager.Instance.GoToScene(1);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameManager.Instance.GoToScene(2, false);
    }

    public void OpenDeadMenu()
    {
        _deadMenuCallCor.Start();
    }

    public void LoadLastSaveButton()
    {
        Time.timeScale = 1f;
        
        GameManager.Instance.GoToScene(1, false);
    }

    private IEnumerator DeadMenuDelayRoutine()
    {
        yield return new WaitForSecondsRealtime(_delayAfterDeadTime);
        
        _title.text = "Умер";
        _continueButton.SetActive(false);
        _returnToBoatButton.SetActive(false);
        _loadLastSaveButton.SetActive(true);
        _restartButton.SetActive(true);

        Pause();
    }
}
