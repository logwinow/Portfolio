using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IncreaseType
{
    Constant = 0,
    Acceleration = 1
}
public class Feature : MonoBehaviour
{
    [SerializeField]
    protected float overloadInc = .2f;
    protected Slider controlSlider;
    protected FeatureParameters? curParam = null;
    
    public FeatureParameters? CurParameters
    {
        get
        {
            return curParam;
        }
        set
        {
            if (curParam == null)
                curParam = value;
        }
    }

    public float CurrentValue
    {
        get
        {
            return controlSlider.value;
        }
    }

    protected virtual void Awake()
    {
        Messenger<bool>.AddListener(MyGameEvents.GAME_END, OnGameEnd);
        controlSlider = GetComponent<Slider>();
    }
    protected virtual void OnDestroy()
    {
        Messenger<bool>.RemoveListener(MyGameEvents.GAME_END, OnGameEnd);
    }

    void OnGameEnd(bool value)
    {
        StopIncrease();
    }
    protected virtual void Update()
    {
        if (controlSlider.value == controlSlider.maxValue)
        {
            if (!(this is OverloadFeature))
                Messenger.Broadcast(MyGameEvents.ATTENTION);
            SceneController.s_overload.AddValue(overloadInc);
        }

        if (CurParameters != null)
        {
            controlSlider.value += CurParameters.Value.Value;
        }
    }

    public virtual void AddValue(float value)
    {
        controlSlider.value += value;

        if (controlSlider.value == controlSlider.minValue)
            curParam = null;
    }

    public void StopIncrease()
    {
        curParam = null;
    }
}

public struct FeatureParameters
{
    public FeatureParameters (IncreaseType mode, float value) : this()
    {
        Mode = mode;
        this.value = value;
    }
    public IncreaseType Mode { get; set; }

    float value;
    public float Value
    {
        get
        {
            if (Mode == IncreaseType.Acceleration)
                value += value * Time.deltaTime;

            return value;
        }
        set
        {
            this.value = value;
        }
    }
}
