using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDiscoveredSound : MonoBehaviour
{
    private bool _isActivated;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActivated && other.CompareTag("Player"))
        {
            AudioManager.Instance.PlayAmbient(-1);
            _isActivated = true;
        }
    }
}
