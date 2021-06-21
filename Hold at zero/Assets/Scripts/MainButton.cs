using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : ControlButton
{
    [SerializeField]
    ControlFeature antifreeze;
    [SerializeField]
    ControlFeature particle;
    [SerializeField]
    Sprite pressed;
    [SerializeField]
    Sprite defaultSprite;

    bool leverPulled = false;
    Image img;

    protected override void Awake()
    {
        base.Awake();

        Messenger<bool>.AddListener(MyGameEvents.LEVER_PULLED, OnLeverPulled);
        img = GetComponent<Image>();
    }

    void OnDestroy()
    {
        Messenger<bool>.RemoveListener(MyGameEvents.LEVER_PULLED, OnLeverPulled);
    }
    protected override void ChangeCurrentFeature()
    {
        if (!sceneController.PlugOFFToggle && sceneController.BrakingToggle)
            antifreeze.feature.AddValue(-antifreeze.DecreaseValue);

        if (leverPulled && !sceneController.TubeToggle && sceneController.BrakingToggle && sceneController.ControlToggle)
        {
            particle.feature.AddValue(-particle.DecreaseValue);
        }

    }

    void OnLeverPulled(bool value)
    {
        leverPulled = value;
    }

    protected override void InitializeKeyPressedEvent()
    {
        keyPressedEvent = MyGameEvents.MAIN_BUTTON_PRESSED;
    }

    protected override void EventAfterPressed()
    {
        base.EventAfterPressed();
        img.sprite = pressed;
    }
    protected override void EventAfterWring()
    {
        base.EventAfterWring();
        img.sprite = defaultSprite;
    }
}
