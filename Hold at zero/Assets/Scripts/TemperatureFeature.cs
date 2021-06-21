using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureFeature : Feature
{
    [SerializeField]
    float bottomLimit = -75f;
    [SerializeField]
    float topLimit = 75f;
    [SerializeField]
    Sprite redColor;
    [SerializeField]
    Sprite blueColor;
    [SerializeField]
    float maxPercentOfColor = .2f;
    [SerializeField]
    Image generatorTemperatureColor;
    public override void AddValue(float value)
    {
        float middleValue = (controlSlider.minValue + controlSlider.maxValue) / 2;
        controlSlider.value += value;


        if (curParam != null)
        {
            if (curParam.Value.Value > 0)
            {
                if (controlSlider.value - value > middleValue && controlSlider.value <= middleValue)
                    curParam = null;
            }
            else if (curParam.Value.Value < 0)
                if (controlSlider.value - value < middleValue && controlSlider.value >= middleValue)
                    curParam = null;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (controlSlider.value == controlSlider.minValue)
            SceneController.s_overload.AddValue(overloadInc);

        if (controlSlider.value >= topLimit)
            Messenger<GeneratorTemperatureType>.Broadcast(MyGameEvents.CHANGE_GENERATOR_VIEW, GeneratorTemperatureType.High);
        else if (controlSlider.value <= bottomLimit)
            Messenger<GeneratorTemperatureType>.Broadcast(MyGameEvents.CHANGE_GENERATOR_VIEW, GeneratorTemperatureType.Low);
        else
            Messenger<GeneratorTemperatureType>.Broadcast(MyGameEvents.CHANGE_GENERATOR_VIEW, GeneratorTemperatureType.Normal);

        if (controlSlider.value > 0 && controlSlider.value < topLimit)
        {
            generatorTemperatureColor.sprite = redColor;
            Color c = generatorTemperatureColor.color;
            c.a = controlSlider.value * maxPercentOfColor / topLimit;
            generatorTemperatureColor.color = c;
        }
        else if (controlSlider.value < 0 && controlSlider.value > bottomLimit)
        {
            generatorTemperatureColor.sprite = blueColor;
            Color c = generatorTemperatureColor.color;
            c.a = Mathf.Abs(controlSlider.value) * maxPercentOfColor / Mathf.Abs(bottomLimit);
            generatorTemperatureColor.color = c;
        }
        else
        {
            Color c = generatorTemperatureColor.color;
            c.a = 0;
            generatorTemperatureColor.color = c;
        }
    }
}
