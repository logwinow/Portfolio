using System;
using System.Collections;
using System.Collections.Generic;
using Custom.SmartCoroutines;
using DitzelGames.PostProcessingTextureOverlay;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int _healthPointCapacity;
    [SerializeField] private GameObject _healthPrefab;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayout;
    [SerializeField] private SectionID _currentSuitParameterID;
    [SerializeField] private SectionID _firstSuitHealthParameterID;
    [SerializeField] private SectionID _secondSuitHealthParameterID;
    [SerializeField] private PostProcessVolume _ppVolume;
    [SerializeField] private float _woundAppearingSpeed;
    

    private int _health;
    private int _maxHealth;
    private SmartCoroutineCache _woundCor;

    public int Health
    {
        get => _health;
        private set => _health = Mathf.Clamp(value, 0, _maxHealth);
    }

    private void Awake()
    {
        _woundCor = new SmartCoroutineCache(this, WoundingRoutine);
        
        GameSaver.onLoadAfterCheck += OnLoad;
    }

    private void OnDestroy()
    {
        GameSaver.onLoadAfterCheck -= OnLoad;
    }

    private void Create(int count)
    {
        while (count-- > 0)
        {
            Instantiate(_healthPrefab, _horizontalLayout.transform);
        }
    }

    private void Remove(int count)
    {
        while (count-- > 0)
        {
            Destroy(_horizontalLayout.transform.GetChild(count).gameObject);
        }
    }

    public void ChangeHealthAt(int value)
    {
        _woundCor.Start();
        
        Health += value;
        
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        int deltaCount = _horizontalLayout.transform.childCount
            - Health / _healthPointCapacity;

        if (deltaCount > 0)
            Remove(deltaCount);
        else if (deltaCount < 0)
            Create(-deltaCount);
    }

    private void OnLoad()
    {
        switch (GameManager.Instance.GetParameter(_currentSuitParameterID).Value)
        {
            case 1:
                _maxHealth = GameManager.Instance.GetParameter(_firstSuitHealthParameterID).Value;  
                break;
            case 2:
                _maxHealth = GameManager.Instance.GetParameter(_secondSuitHealthParameterID).Value;
                break;
            default:
                Debug.LogError("Incorrect current suit index");
                break;
        }

        _health = _maxHealth;
        
        Create(_health / _healthPointCapacity);
    }

    private IEnumerator WoundingRoutine()
    {
        float a = 0;
        var texOverlay = _ppVolume.profile.GetSetting<TextureOverlay>();

        while ((a += Time.deltaTime * _woundAppearingSpeed) < 1f)
        {
            texOverlay.transparency.value = a;

            yield return null;
        }
        
        while ((a -= Time.deltaTime * _woundAppearingSpeed) > 0)
        {
            texOverlay.transparency.value = a;

            yield return null;
        }

        texOverlay.transparency.value = 0;
    }
}
