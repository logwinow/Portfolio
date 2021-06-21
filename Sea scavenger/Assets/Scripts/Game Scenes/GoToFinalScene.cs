using System;
using System.Collections;
using System.Collections.Generic;
using Custom.SmartCoroutines;
using UnityEngine;

public class GoToFinalScene : MonoBehaviour
{
    private bool _isActivated;
    
    private void OnTriggerEnter2D(Collider2D other)
    {    
        if (!_isActivated && other.CompareTag("Player"))
        {
            new SmartWaitingCoroutine(this).Start(1, 
                methodAfter: 
                () => GameManager.Instance
                    .GoToScene(3, false, false));
            
            _isActivated = true;
        }
    }
}
