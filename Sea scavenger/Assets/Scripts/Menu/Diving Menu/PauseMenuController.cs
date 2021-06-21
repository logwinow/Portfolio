using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    protected Func<bool> callCondition;
    
    public bool IsPaused { get; private set; }

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (callCondition?.Invoke() ?? true))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
        
        IsPaused = true;
    }

    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        
        IsPaused = false;
    }
}
