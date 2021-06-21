using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.SmartCoroutines;
using UnityEngine.Rendering.PostProcessing;

public class OxygenController : MonoBehaviour
{
    [SerializeField] private Image _imgFill;
    [ReadOnly]
    [SerializeField] private float _oxygen;
    [SerializeField] private float _oxygenConsumption;
    [SerializeField] private SectionID _oxygenAmountParameter;
    [SerializeField] private PostProcessVolume _ppVolume;
    [Range(0, 1f)] [SerializeField] private float _lowOxygenFraction; 
    
    private SmartCoroutineCache _consumptionCor;
    private float _startOxygenAmount;

    private void Awake()
    {
        _consumptionCor = new SmartCoroutineCache(this, ConsumptionRoutine); 
    }

    private void Start()
    {
        _oxygen = GameManager.Instance.GetParameter(_oxygenAmountParameter).Value;
        _startOxygenAmount = _oxygen;
        StartConsumption();
    }

    public void StartConsumption()
    {
        _consumptionCor.Start();
    }

    public void StopConsumption()
    {
        _consumptionCor.Stop();
    }

    private IEnumerator ConsumptionRoutine()
    {
        while (_oxygen >= 0)
        {
            Consumpt(_oxygenConsumption);

            yield return null;

            if (PlayerController.Instance != null && PlayerController.Instance.IsDead)
            {
                yield break;
            }
        }
        
        PlayerController.Instance.AnimationController.DBAnimationController
            .SetBool("IsSuffocated", true);
        PlayerController.Instance.Kill();
        
        AudioManager.Instance.PlayOneShot(AudioManager.DEATH_BY_SUFFOCATED);
    }

    public void Consumpt(float amount)
    {
        _oxygen -= amount * Time.deltaTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        var fract = _oxygen / _startOxygenAmount;
        _imgFill.fillAmount = fract;

        if (fract <= _lowOxygenFraction)
        {
            var fractOfFract = 1 - fract / _lowOxygenFraction;
            
            if (_ppVolume.profile.TryGetSettings(out ColorGrading cg))
            {
                cg.saturation.value = Mathf.Lerp(0, -80, fractOfFract);
            }

            if (_ppVolume.profile.TryGetSettings(out Grain gr) 
                && _ppVolume.sharedProfile.TryGetSettings(out Grain grOrigin))
            {
                gr.size.value = Mathf.Lerp(grOrigin.size, 1f, fractOfFract);
                gr.intensity.value = Mathf.Lerp(grOrigin.intensity, 1f, fractOfFract);
            }
        }
    }
}
