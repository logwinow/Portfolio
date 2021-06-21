using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplayer : MonoBehaviour, IReusable
{
    [SerializeField] private TextMeshPro _damageTMP; 
    [SerializeField] private HealthController _healthController;
    [SerializeField] private float _textDelay;

    private Coroutine _textDelayCoroutine;

    private void Awake()
    {
        _healthController.onDamage += OnDamage;
    }

    private void OnDestroy()
    {
        _healthController.onDamage -= OnDamage;
    }

    private void Start()
    {
        _damageTMP.gameObject.SetActive(false);
    }

    public void OnDamage(float damage)
    {
        RefreshText(damage);
        SetActiveText(true);
        
        if (_textDelayCoroutine != null)
            StopCoroutine(_textDelayCoroutine);
        _textDelayCoroutine = StartCoroutine(TextDelayCoroutine());
    }

    private void RefreshText(float damage)
    {
        _damageTMP.text = $"-{damage}";
    }

    private void SetActiveText(bool value)
    {
        _damageTMP.gameObject.SetActive(value);
    }

    private IEnumerator TextDelayCoroutine()
    {
        float t = 0;

        while ((t += Time.deltaTime) < _textDelay)
        {
            _damageTMP.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            
            yield return null;
        }
        
        SetActiveText(false);

        _textDelayCoroutine = null;
    }

    public void Reuse()
    {
        SetActiveText(false);
    }
}
